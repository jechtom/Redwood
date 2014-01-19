using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Redwood.Framework.Binding
{
    /// <summary>
    /// Base class for all classes that supports binding.
    /// </summary>
    public abstract class RedwoodBindable
    {
        private Thread originThread;

        private List<ValueEntry> localValues;

        private RedwoodBindable parent;
        
        public RedwoodBindable()
        {
            originThread = Thread.CurrentThread;
        }

        public object GetValue(RedwoodProperty property)
        {
            VerifyAccess();

            ValueEntry entry = GetValueDirect(property, true);
            return entry.Value;
        }

        public object GetRawValue(RedwoodProperty property)
        {
            VerifyAccess();

            ValueEntry entry = GetValueDirect(property, false);
            return entry.Value;
        }

        public void SetValue(RedwoodProperty property, object value)
        {
            VerifyAccess();

            property.ValidatePropertyValue(value);

            var itemRef = TryFindLocalValue(property.Id);
            UpdateLocalValue(itemRef, new ValueEntry()
            {
                PropertyId = property.Id,
                Value = value
            });
        }

        public void ClearValue(RedwoodProperty property)
        {
            VerifyAccess();

            var itemRef = TryFindLocalValue(property.Id);

            ClearLocalValue(itemRef);
        }

        protected void SetParent(RedwoodBindable parent)
        {
            VerifyAccess();
            this.parent = parent;
        }

        protected void VerifyAccess()
        {
            if (System.Threading.Thread.CurrentThread != originThread)
                throw new InvalidOperationException("Cross-thread access not allowed.");
        }

        private ValueEntryRef TryFindLocalValue(int propertyId)
        {
            if (localValues == null || localValues.Count == 0)
                return ValueEntryRef.NotFound;

            for (int i = 0; i < localValues.Count; i++)
            {
                if (localValues[i].PropertyId == propertyId)
                    return new ValueEntryRef() { Found = true, Index = i };
            }

            return ValueEntryRef.NotFound;
        }

        private void UpdateLocalValue(ValueEntryRef entryRef, ValueEntry entry)
        {
            if (localValues == null)
                localValues = new List<ValueEntry>();

            if (!entryRef.Found)
            {
                // new local value
                localValues.Add(entry);
            }
            else
            {
                // update
                localValues[entryRef.Index] = entry;
            }
        }

        private void ClearLocalValue(ValueEntryRef entryRef)
        {
            if (localValues == null)
                return;

            if (entryRef.Found)
            {
                // remove
                localValues.RemoveAt(entryRef.Index);
            }
        }

        private ValueEntry GetValueDirect(RedwoodProperty property, bool resolveExpressions)
        {
            var itemRef = TryFindLocalValue(property.Id);
            
            if(itemRef.Found)
            {
                // local value
                var localValue = GetLocalEntry(itemRef);
                if (localValue.Value is BindingBase && resolveExpressions)
                {
                    // evaluate expression
                    var localValueExpression = (BindingBase)localValue.Value;
                    localValue.Value = localValueExpression.Evaluate(this);
                }
                return localValue;
            }

            if(property.Metadata.IsInherited && parent != null)
            {
                // inherited value
                return parent.GetValueDirect(property, resolveExpressions);
            }

            // default value
            return new ValueEntry()
            {
                PropertyId = property.Id,
                Value = property.Metadata.DefaultValue
            };
        }

        private ValueEntry GetLocalEntry(ValueEntryRef entryRef)
        {
            if (!entryRef.Found)
                throw new InvalidOperationException("Entry not found.");

            return localValues[entryRef.Index];
        }

        private struct ValueEntryRef
        {
            public static ValueEntryRef NotFound = new ValueEntryRef() { Found = false, Index = -1 };

            public int Index;
            public bool Found;
        }

        private struct ValueEntry
        {
            public int PropertyId;
            public object Value;
        }
    }
}
