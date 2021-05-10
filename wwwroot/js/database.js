window.context = {
    db: null,
    justUpgraded: false,
    currentVersion: 1
};

function DataUpgrade(dotnetReference)
{
    console.log("Data upgrade started");
    console.log("dbVersion:" + context.db.version);
    
    switch (context.db.version)
    {
        case 1:
            let book = { id: 'js', price: 10 };

            let transaction = context.db.transaction("books", "readwrite");
            transaction.objectStore("books").add(book);

            transaction.oncomplete = function () {
                console.log("Транзакция выполнена");
                dotnetReference.invokeMethod('SetStatus', true);
            };

            transaction.onerror = function (e) {
                console.log("Транзакция не выполнена." + e.error);
                dotnetReference.invokeMethod('SetStatus', false);
                e.stopPropagation();
            };
            break;

        default:
            console.log("no data upgrade script for current db version");
            break;
    }
}

function SchemaUpgrade() {

    console.log("Schema upgrade started");
    console.log("dbVersion:" + context.db.version);
    let upgradeWasNotSuccess = false;

    switch (context.db.version) {
        case 1:
            context.db.createObjectStore('books', { keyPath: 'id' });
            break;

        default:
            upgradeWasNotSuccess = true;
            console.log("no schema upgrade script for current db version");
            break;
    }

    if (!upgradeWasNotSuccess) {
        context.justUpgraded = true;
    }
}


window.database = {

    initDatabase: function (dotnetReference) {
        console.log("Database initialization started");

        indexedDB.deleteDatabase("db");
        let openRequest = indexedDB.open("db", 1);

        openRequest.onerror = function () {
            console.error("Error", openRequest.error);
            dotnetReference.invokeMethod('SetStatus', false);
        };

        openRequest.onsuccess = function () {
            context.db = openRequest.result;
            if (context.justUpgraded) {
                DataUpgrade(dotnetReference);
            }
            else {
                dotnetReference.invokeMethod('SetStatus', true);
            }
            console.log("Database initialization finished");
        };

        openRequest.onupgradeneeded = function () {
            context.db = openRequest.result;
            SchemaUpgrade();
        }
    },
    getVerseById: function (dotnetCallback, bookId, verseId) {
        let result = "Bible verse stub: " + bookId + ":" + verseId;
    },
    displayWelcome: function (welcomeMessage) {
        document.getElementById('welcome').innerText = welcomeMessage;
    },
    returnArrayAsyncJs: function () {
        DotNet.invokeMethodAsync('BlazorWebAssemblySample', 'ReturnArrayAsync')
            .then(data => {
                data.push(4);
                console.log(data);
            });
    },
    jsLog: function (text) { console.log(text);},
    showPrompt: function (text) {
        return prompt(text, 'Type your name here');
    },
    sayHello: function (dotnetHelper) {
        return dotnetHelper.invokeMethodAsync('SayHello')
            .then(r => console.log(r));
    },
    test: function (dotnetHelper) {
        var openRequest = window.indexedDB.open("db", context.currentVersion);

        openRequest.onsuccess = function (event) {
            context.db = openRequest.result;
            var transaction = context.db.transaction("books", "readonly");
            transaction.oncomplete = function (event) {
                //
            };
            transaction.onerror = function (event) {
                //
            };
            var objectStore  = transaction.objectStore("books");
            var objectStoreRequest = objectStore.get('js');
            objectStoreRequest.onsuccess = function (event) {
                console.log("got result " + result);
                var result = objectStoreRequest.result.price;
                dotnetHelper.invokeMethod('SetStatusAndResult', true, result);
            };
        };
    }
};
