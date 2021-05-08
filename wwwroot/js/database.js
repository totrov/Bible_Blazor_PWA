window.context = {
    db: null,
    justUpgraded: false
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
                dotnetReference.invokeMethod('SetResult', true);
            };

            transaction.onerror = function (e) {
                console.log("Транзакция не выполнена." + e.error);
                dotnetReference.invokeMethod('SetResult', false);
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

        //indexedDB.deleteDatabase("store");
        let openRequest = indexedDB.open("db", 1);

        openRequest.onerror = function () {
            console.error("Error", openRequest.error);
            dotnetReference.invokeMethod('SetResult', false);
        };

        openRequest.onsuccess = function () {
            context.db = openRequest.result;
            if (context.justUpgraded) {
                DataUpgrade(dotnetReference);
            }
            else {
                dotnetReference.invokeMethod('SetResult', true);
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
    }
};
