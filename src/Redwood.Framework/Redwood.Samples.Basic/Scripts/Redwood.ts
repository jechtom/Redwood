class Redwood
{

    static ViewModels: any[] = [];


    static CreateViewModel(data: any, target: any): any {
        return ko.mapping.fromJS(data, {}, target);
    }

    static PostBack(currentDataContextPath: string, commandName: string, commandArguments: any[]): any {
        // unwrap arguments
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
        Redwood.SendJSONByAjax(document.location.href, "POST", postData, xhr => {
            if (xhr.status == 200) {
                // map the view model
                ko.mapping.fromJSON(xhr.responseText, {}, viewModel);
            }
            else if (xhr.status == 304) {
                // redirect
                document.location.href = xhr.getResponseHeader("Location");
            }
        }, xhr => {
            alert('Error');
        });
    }

    static SendJSONByAjax(url: string, method: string, postData: any, success: (request: XMLHttpRequest) => void, error: (response: XMLHttpRequest) => void) {
        var xhr = XMLHttpRequest ? new XMLHttpRequest() : <XMLHttpRequest>new ActiveXObject("Microsoft.XMLHTTP");
        xhr.open(method, url, true);
        xhr.setRequestHeader("Content-Type", "application/json");
        xhr.onreadystatechange = () => {
            if (xhr.readyState != 4) return;
            if (xhr.status < 400) {
                success(xhr);
            } else {
                error(xhr);
            }
        };
        xhr.send(JSON.stringify(postData));
    }


}