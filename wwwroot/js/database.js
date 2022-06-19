window.context = {
    db: null,
    justUpgraded: false,
    currentVersion: 4,
    previousVersion: 0,
    dbName: 'db',
    debugMode: true,
    verbose: true,
    log: function (message) { if (this.debugMode) console.log(message); },
    logVerbose: function (message) { if (this.debugMode && this.verbose) console.log(message); },
    dataUpgradeAlreadyStarted: false
};

function DataUpgrade() {
    if (window.context.dataUpgradeAlreadyStarted)
        return;
    window.context.dataUpgradeAlreadyStarted = true;
    console.log("Data upgrade started");
    console.log("dbVersion:" + context.db.version + " prevVersion:" + context.previousVersion);

    switch (context.previousVersion) {
        case 0:
            database.dataUpgradeFunctions[0]();
            database.dataUpgradeFunctions[1]();
            database.dataUpgradeFunctions[2]();
            break;
        case 1:
            database.dataUpgradeFunctions[1]();
            database.dataUpgradeFunctions[2]();
            break;
        case 2:
        case 3:
            database.dataUpgradeFunctions[2]();
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
            database.schemaUpgradeFunctions[2]();
            break;
        case 1:
        case 2:
            database.schemaUpgradeFunctions[1]();
            database.schemaUpgradeFunctions[2]();
            break;
        case 3:
            database.schemaUpgradeFunctions[2]();
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

    getOpenRequest: function (dotnetReference) {

        console.log("Open db request");
        let openRequest = indexedDB.open("db", context.currentVersion);

        openRequest.onerror = function () {
            console.error("Open request error", openRequest.error);
        };

        openRequest.onSuccessHandlers = [];
        openRequest.onsuccess = function (e) {

            this.onSuccessHandlers.filter(v => typeof v === 'function').forEach(f => { f(e) });
        };
        openRequest.onSuccessHandlers.push(function (e) {
            context.db = openRequest.result;
            if (context.justUpgraded && !context.dataUpgradeAlreadyStarted) {
                DataUpgrade();
            }
        });

        openRequest.onupgradeneeded = function (e) {
            context.db = openRequest.result;
            context.previousVersion = e.oldVersion;
            SchemaUpgrade();
        }
        return openRequest;
    },
    getVerseById: function (dotnetCallback, bookId, verseId) {
        let result = "Bible verse stub: " + bookId + ":" + verseId;
    },
    jsLog: function (text) { console.log(text); },
    jsAlert: function (text) { alert(text); },
    getRecordFromObjectStoreByKey: function (dotnetHelper, params) {
        context.log('getRecordFromObjectStoreByKey was called');
        var openRequest = database.getOpenRequest();

        openRequest.onSuccessHandlers.push(function (event) {
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
                context.logVerbose('getRecordFromObjectStoreByKey: Transaction returned for key ' + key + ':' + JSON.stringify(result));
                dotnetHelper.invokeMethod('SetStatusAndResult', true, result);
            };
        });

        openRequest.onerror = function (event) {
            context.log('getRecordFromObjectStoreByKey: Database not opened due to error: ' + openRequest.error);
            dotnetHelper.invokeMethod('SetStatusAndResult', false, null);
        }
    },
    getRecordFromObjectStoreByIndex: function (dotnetHelper, params) {
        context.log('getRecordFromObjectStoreByIndex was called');
        var openRequest = database.getOpenRequest();

        openRequest.onSuccessHandlers.push(function (event) {
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
        });
    },
    getAllFromObjectStore: function (dotnetHelper, objectStoreName) {
        context.log('getAllFromObjectStore was called');
        var openRequest = database.getOpenRequest();

        openRequest.onSuccessHandlers.push(function (event) {
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
        });
    },
    getRangeFromObjectStoreByKey: function (dotnetHelper, params) {
        context.log('getRangeFromObjectStoreByKey was called');
        var openRequest = database.getOpenRequest();

        openRequest.onSuccessHandlers.push(function (event) {
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
        });
    },
    putKeyValueIntoObjectStore: function (dotnetHelper, params) {
        context.log('putKeyValueIntoObjectStore was called with params:' + params.join());
        var openRequest = database.getOpenRequest();

        openRequest.onSuccessHandlers.push(function (event) {
            context.log('db opened');
            context.db = openRequest.result;
            let objectStoreName = params.shift();
            var transaction = context.db.transaction(objectStoreName, "readwrite");

            transaction.oncomplete = function (event) {
                context.log('putKeyValueIntoObjectStore: Transaction completed.');
            };

            transaction.onerror = function (event) {
                context.log('putKeyValueIntoObjectStore: Transaction not opened due to error: ' + transaction.error);
            };

            var objectStore = transaction.objectStore(objectStoreName);
            var key = params.shift();
            var value = params.shift();
            var objectStoreRequest = objectStore.put({ Key: key, Value: value });
            objectStoreRequest.onsuccess = function (event) {
                result = true;
                context.logVerbose('putKeyValueIntoObjectStore: Transaction returned: ' + result);
                dotnetHelper.invokeMethod('SetStatusAndResult', true, result);
            };

            objectStoreRequest.onerror = function (event) {
                result = false;
                context.logVerbose('putKeyValueIntoObjectStore: Transaction returned: ' + result);
                dotnetHelper.invokeMethod('SetStatusAndResult', false, result);
            };
        });
    },
    getCountFromObjectStoreByKey: function (dotnetHelper, params) {
        context.log('getCountFromObjectStoreByKey was called');
        var openRequest = database.getOpenRequest();

        openRequest.onSuccessHandlers.push(function (event) {
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
            var objectStoreRequest = objectStore.count(key);

            objectStoreRequest.onsuccess = function (event) {
                result = objectStoreRequest.result;
                context.logVerbose('getCountFromObjectStoreByKey: Transaction returned: ' + result);
                dotnetHelper.invokeMethod('SetStatusAndResult', true, result);
            };
        });

        openRequest.onerror = function (event) {
            context.log('getRecordFromObjectStoreByKey: Database not opened due to error: ' + openRequest.error);
            dotnetHelper.invokeMethod('SetStatusAndResult', false, null);
        }
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
            context.db.createObjectStore('parameters', { keyPath: ['Key'] });
        },
        function /*2*/() {
            //do nothing. New version bd is for cleaning up the lessons object store by cause of new id algorithm was introduced.
        }
    ],
    fetchJson: async (path, dbStore) => {
        const response = await fetch(path);
        var json = await response.json();
        var transaction = context.db.transaction(dbStore, "readwrite");
        var os = transaction.objectStore(dbStore);
        json.forEach(function (data) { os.add(data); });

        transaction.oncomplete = function () {
            console.log(dbStore + ' ' + 'fetch transaction completed');
        };

        transaction.onerror = function (e) {
            console.log(dbStore + ' ' + 'fetch transaction failed. ' + transaction.error);
            dotnetReference.invokeMethod('SetStatus', false);
            e.stopPropagation();
        };
    },
    importJson: async (dotnetHelper, jsonString, dbStore) => {
        var transaction = context.db.transaction(dbStore, "readwrite");
        var os = transaction.objectStore(dbStore);
        var json = JSON.parse(jsonString);
        json.forEach(function (data) { os.put(data); });

        transaction.oncomplete = function () {
            console.log(dbStore + ' ' + 'import transaction completed for ' + json[0].UnitId);
            dotnetHelper.invokeMethod('SetStatus', true);
        };

        transaction.onerror = function (e) {
            console.log(dbStore + ' ' + 'import transaction failed for ' + json[0].UnitId + ': ' + transaction.error);
            dotnetHelper.invokeMethod('SetStatus', false);
            e.stopPropagation();
        };
    },
    dataUpgradeFunctions: [
        function /*0*/() {
            database.fetchJson('/Assets/books.json', 'books');
            database.fetchJson('/Assets/verses.json', 'verses');
        },
        function /*1*/() {
            database.fetchJson('/Assets/lessonUnits.json', 'lessonUnits');
        },
        function /*2*/() {
            var transaction = context.db.transaction("lessons", "readwrite");
            var os = transaction.objectStore("lessons");
            os.clear();
            transaction.oncomplete = function (event) {
                console.log("lessons store was cleaned");
            };

            transaction.onerror = function (event) {
                console.log("clean of lessons sotre failed: " + transaction.error);
            };
        }
    ],
};