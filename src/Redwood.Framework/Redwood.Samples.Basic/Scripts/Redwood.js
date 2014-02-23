var Redwood = (function () {
    function Redwood() {
    }
    Redwood.CreateViewModel = function (data, target) {
        return ko.mapping.fromJS(data, {}, target);
    };

    Redwood.PostBack = function (currentDataContextPath, commandName, commandArguments) {
        for (var arg in commandArguments) {
            commandArguments[arg] = ko.mapping.toJS(commandArguments[arg]);
        }

        // get view model
        var viewModel = Redwood.ViewModels["Default"];
        var postData = {
            viewModel: ko.mapping.toJS(viewModel),
            commandName: commandName,
            commandTarget: currentDataContextPath,
            commandArguments: commandArguments
        };

        // send the request
        Redwood.SendJSONByAjax(document.location.href, "POST", postData, function (xhr) {
            if (xhr.status == 200) {
                // map the view model
                ko.mapping.fromJSON(xhr.responseText, {}, viewModel);
            } else if (xhr.status == 304) {
                // redirect
                document.location.href = xhr.getResponseHeader("Location");
            }
        }, function (xhr) {
            alert('Error');
        });
    };

    Redwood.SendJSONByAjax = function (url, method, postData, success, error) {
        var xhr = XMLHttpRequest ? new XMLHttpRequest() : new ActiveXObject("Microsoft.XMLHTTP");
        xhr.open(method, url, true);
        xhr.setRequestHeader("Content-Type", "application/json");
        xhr.onreadystatechange = function () {
            if (xhr.readyState != 4)
                return;
            if (xhr.status < 400) {
                success(xhr);
            } else {
                error(xhr);
            }
        };
        xhr.send(JSON.stringify(postData));
    };
    Redwood.ViewModels = [];
    return Redwood;
})();
//# sourceMappingURL=Redwood.js.map
