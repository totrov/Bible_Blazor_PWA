function initDatabase(books) {

    let openRequest = indexedDB.open("store", 1);

    openRequest.onerror = function () {
        console.error("Error", openRequest.error);
    };

    openRequest.onsuccess = function () {
        let book = { id: 'js', price: 10 };

        let db = openRequest.result;
        let transaction = db.transaction("books", "readwrite");
        let request = transaction.objectStore("books").add(book);

        transaction.oncomplete = function () {
            console.log("Транзакция выполнена");
        };
    };

    openRequest.onupgradeneeded = function () {
        // версия существующей базы данных меньше 2 (или база данных не существует)
        let db = openRequest.result;
        switch (db.version) { // существующая (старая) версия базы данных
            case 0:
                db.createObjectStore('books', { keyPath: 'id' });
                //db.createObjectStore('verses', { keyPath: 'id' });
                break;
            case 1:
                // на клиенте версия базы данных 1
                // обновить
                break;
            default:
                break;

        }
    }
}

function getVerseById(dotnetCallback, bookId, verseId) {
    let result = "Bible verse stub: " + bookId + ":" + verseId;
}