using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Redwood.Samples.Basic
{
    public class UserProfileControlPresenter
    {
        public void Get()
        {
            //return Page("UserProfileControl.rwhtml");
        }

        public void SaveChanges(UserProfileViewModel model)
        {

        }

        public void CheckBirthDate(DateTime birthDate)
        {

        }
    }

    public class UserProfileViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
    }

    /* 
    USERPROFILECONTROL.RWHTML
    
    <div class="userprofile">
      <rw:TextBox Text="@.FirstName" />
      <rw:TextBox Text="@.LastName" />
      <my:DateTimeEdit Value="@.BirthDate" />
      <rw:Button OnClick="@>CheckBirthDate(.BirthDate)" Text="Kolik je mi let?" />
      <rw:Button OnClick="@>SaveChanges(.)" Text="Změnit" />
    </div>
     
      
    */
}