window.context = {
    db: null,
    justUpgraded: false,
    currentVersion: 2,
    previousVersion: 0,
    dbName: 'db',
    debugMode: true,
    verbose: true,
    log: function (message) { if (this.debugMode) console.log(message); },
    logVerbose: function (message) { if (this.debugMode && this.verbose) console.log(message); }
};

function DataUpgrade(dotnetReference) {
    console.log("Data upgrade started");
    console.log("dbVersion:" + context.db.version + " prevVersion:" + context.previousVersion);

    switch (context.previousVersion) {
        case 0:
            database.dataUpgradeFunctions[0](dotnetReference);
            database.dataUpgradeFunctions[1](dotnetReference);
            break;
        case 1:
            database.dataUpgradeFunctions[1](dotnetReference);
            break;
        default:
            console.log("no data upgrade script for current db version");
            break;
    }
}

function SchemaUpgrade() {

    console.log("Schema upgrade started");
    console.log("dbVersion:" + context.db.version + " prevVersion:" + context.previousVersion);
    let upgradeWasNotSuccess = false;

    switch (context.previousVersion) {
        case 0:
            database.schemaUpgradeFunctions[0]();
            database.schemaUpgradeFunctions[1]();
            break;
        case 1:
            database.schemaUpgradeFunctions[1]();
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

    delete: function () {
        if (prompt("Удаление базы необратимо! Чтобы удалить введите слово 'delete' без кавычек") == 'delete') {
            var req = indexedDB.deleteDatabase("db");
            req.onsuccess = function () {
                console.log("Deleted database successfully");
            };
            req.onerror = function () {
                console.log("Couldn't delete database");
            };
            req.onblocked = function () {
                console.log("Couldn't delete database due to the operation being blocked");
            };
        }
    },

    initDatabase: function (dotnetReference) {
        //indexedDB.deleteDatabase("db");

        console.log("Database initialization started");
        let openRequest = indexedDB.open("db", context.currentVersion);

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
                console.log("Database initialization finished");
            }
        };

        openRequest.onupgradeneeded = function (e) {
            context.db = openRequest.result;
            context.previousVersion = e.oldVersion;
            SchemaUpgrade();
        }
    },
    getVerseById: function (dotnetCallback, bookId, verseId) {
        let result = "Bible verse stub: " + bookId + ":" + verseId;
    },
    jsLog: function (text) { console.log(text); },
    jsAlert: function (text) { alert(text); },
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
            var key = params.length > 1 ? params : params.shift();
            var objectStoreRequest = objectStore.get(key);
            objectStoreRequest.onsuccess = function (event) {
                result = objectStoreRequest.result;
                context.logVerbose('getRecordFromObjectStoreByKey: Transaction returned: ' + result);
                dotnetHelper.invokeMethod('SetStatusAndResult', true, result);
            };
        };

        openRequest.onerror = function (event) {
            context.log('getRecordFromObjectStoreByKey: Database not opened due to error: ' + openRequest.error);
        }
    },
    getRecordFromObjectStoreByIndex: function (dotnetHelper, params) {
        context.log('getRecordFromObjectStoreByIndex was called');
        var openRequest = window.indexedDB.open(context.dbName, context.currentVersion);

        openRequest.onsuccess = function (event) {
            context.log('db opened');
            context.db = openRequest.result;
            let objectStoreName = params.shift();
            var transaction = context.db.transaction(objectStoreName, "readonly");

            transaction.oncomplete = function (event) {
                context.log('getRecordFromObjectStoreByIndex: Transaction completed.');
            };

            transaction.onerror = function (event) {
                context.log('getRecordFromObjectStoreByIndex: Transaction not opened due to error: ' + transaction.error);
            };

            var objectStore = transaction.objectStore(objectStoreName);
            var index = objectStore.index(params.shift());
            var key = params.length > 1 ? params : params.shift();
            var indexRequest = index.get(key);
            indexRequest.onsuccess = function (event) {
                result = indexRequest.result;
                context.logVerbose('getRecordFromObjectStoreByIndex: Transaction returned: ' + result);
                dotnetHelper.invokeMethod('SetStatusAndResult', true, result);
            };
        };
    },
    getAllFromObjectStore: function (dotnetHelper, objectStoreName) {
        context.log('getAllFromObjectStore was called');
        var openRequest = window.indexedDB.open(context.dbName, context.currentVersion);

        openRequest.onsuccess = function (event) {
            context.log('db opened');
            context.db = openRequest.result;
            var transaction = context.db.transaction(objectStoreName, "readonly");

            transaction.oncomplete = function (event) {
                context.log('getRecordFromObjectStoreByKey: Transaction completed.');
            };

            transaction.onerror = function (event) {
                context.log('getRecordFromObjectStoreByKey: Transaction not opened due to error: ' + transaction.error);
            };

            var objectStore = transaction.objectStore(objectStoreName);

            var objectStoreRequest = objectStore.getAll();
            objectStoreRequest.onsuccess = function (event) {
                result = objectStoreRequest.result;
                context.logVerbose('getAllFromObjectStore: Transaction returned: ' + result);
                dotnetHelper.invokeMethod('SetStatusAndResult', true, result);
            };
        };
    },
    getRangeFromObjectStoreByKey: function (dotnetHelper, params) {
        context.log('getRangeFromObjectStoreByKey was called');
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
            var parameters = params[0];
            var upperBoundLastSubKey = parameters.pop();
            var lowerBound, upperBound;
            if (parameters.length > 1) {
                lowerBound = parameters;
                upperBound = parameters.slice(0, -1);
                upperBound.push(upperBoundLastSubKey);
            }
            else {
                lowerBound = parameters.shift();
                upperBound = upperBoundLastSubKey;
            }

            var objectStoreRequest = objectStore.getAll(IDBKeyRange.bound(lowerBound, upperBound));
            objectStoreRequest.onsuccess = function (event) {
                result = objectStoreRequest.result;
                context.logVerbose('getRecordFromObjectStoreByKey: Transaction returned: ' + result);
                dotnetHelper.invokeMethod('SetStatusAndResult', true, result);
            };
        };
    },
    schemaUpgradeFunctions: [
        function (dotnetHelper) {
            var booksObjectStore = context.db.createObjectStore('books', { keyPath: 'Id' });
            booksObjectStore.createIndex("ShortName", "ShortName", { unique: true });
            context.db.createObjectStore('verses', { keyPath: ['BookId', 'Chapter', 'Id'] });
            context.db.createObjectStore('lessonUnits', { keyPath: ['Id'] });
            context.db.createObjectStore('lessons', { keyPath: ['UnitId', 'Id'] });
        },
        function /*1*/() {
            console.log("nothing to upgrade in schema");
        }
    ],
    fetchJson: async (path, dbStore, dotnetReference) => {
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
            console.log(dbStore + ' ' + 'fetch transaction failed. ' + transaction.error);
            dotnetReference.invokeMethod('SetStatus', false);
            e.stopPropagation();
        };
    },
    dataUpgradeFunctions: [
        function /*0*/(dotnetReference) {
            database.fetchJson('/Assets/books.json', 'books', dotnetReference);
            database.fetchJson('/Assets/verses.json', 'verses', dotnetReference);
        },
        function /*1*/(dotnetReference) {
            database.fetchJson('/Assets/lessonUnits.json', 'lessonUnits', dotnetReference);
            database.fetchJson('/Assets/lessons/Byt.json', 'lessons', dotnetReference).then(console.log("Byt initialization finished"));
            database.fetchJson('/Assets/lessons/Evn.json', 'lessons', dotnetReference).then(console.log("Evn initialization finished"));
            database.fetchJson('/Assets/lessons/Osn.json', 'lessons', dotnetReference).then(console.log("Database initialization finished"));
            //database.fetchJson('/Assets/lessons/IskhSol.json', 'lessons', dotnetReference)
        }
    ],
};