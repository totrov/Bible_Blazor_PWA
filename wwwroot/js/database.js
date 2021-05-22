window.context = {
    db: null,
    justUpgraded: false,
    currentVersion: 1,
    dbName: 'db',
    debugMode: true,
    verbose: true,
    log: function (message) { if (this.debugMode) console.log(message); },
    logVerbose: function (message) { if (this.debugMode && this.verbose) console.log(message); }
};

function DataUpgrade(dotnetReference)
{
    console.log("Data upgrade started");
    console.log("dbVersion:" + context.db.version);
    
    switch (context.db.version)
    {
        case 1:
            var fetchJson = async (path, dbStore) => {
                const response = await fetch(path);
                var json = await response.json();
                var transaction = context.db.transaction(dbStore, "readwrite");
                var os = transaction.objectStore(dbStore);
                json.forEach(function (data) { os.add(data); });

                transaction.oncomplete = function () {
                    console.log(dbStore + ' ' + 'fetch transaction completed');
                    dotnetReference.invokeMethod('SetStatus', true);
                };

                transaction.onerror = function (e) {
                    console.log(dbStore + ' ' + 'fetch transaction failed. ' + e.error);
                    dotnetReference.invokeMethod('SetStatus', false);
                    e.stopPropagation();
                };
            };
            fetchJson('/Assets/books.json', 'books');
            fetchJson('/Assets/verses.json', 'verses');

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
            context.db.createObjectStore('books', { keyPath: 'Id' });
            context.db.createObjectStore('verses', { keyPath: ['BookId', 'Chapter', 'Id'] });
            context.db.createObjectStore('lessonUnits', { keyPath: ['Id'] });
            context.db.createObjectStore('lessons', { keyPath: ['UnitId', 'Id'] });
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
    jsLog: function (text) { console.log(text);},
    showPrompt: function (text) {
        return prompt(text, 'Type your name here');
    },
    getRecordFromObjectStoreByKey: function (dotnetHelper, params) {
        context.log('getRecordFromObjectStoreByKey was called');
        var openRequest = window.indexedDB.open(context.dbName, context.currentVersion);

        openRequest.onsuccess = function (event) {
            context.log('db opened');
            context.db = openRequest.result;
            let objectStoreName = params.shift();
            var transaction = context.db.transaction(objectStoreName, "readonly");

            transaction.oncomplete = function (event) {
                context.log('getRecordFromObjectStoreByKey: Transaction completed.');
            };

            transaction.onerror = function (event) {
                context.log('getRecordFromObjectStoreByKey: Transaction not opened due to error: ' + transaction.error);
            };

            var objectStore = transaction.objectStore(objectStoreName);
            var objectStoreRequest = objectStore.get(params.shift());
            objectStoreRequest.onsuccess = function (event) {
                result = objectStoreRequest.result;
                context.logVerbose('getRecordFromObjectStoreByKey: Transaction returned: ' + result);
                dotnetHelper.invokeMethod('SetStatusAndResult', true, result);
            };
        };
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
                context.log("got result " + result);
                var result = objectStoreRequest.result.price;
                dotnetHelper.invokeMethod('SetStatusAndResult', true, result);
            };
        };
    }
};