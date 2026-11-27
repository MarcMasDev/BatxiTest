// Descarga un texto como archivo .ics. En el iPhone, Safari lo abre como evento con el botón "Añadir" al Calendario
mergeInto(LibraryManager.library, {
  DownloadCalendarFile: function (fileName, content) {
    var name = UTF8ToString(fileName);
    var text = UTF8ToString(content);

    var blob = new Blob([text], { type: "text/calendar;charset=utf-8" });
    var url = URL.createObjectURL(blob);

    var link = document.createElement("a");
    link.href = url;
    link.download = name;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    setTimeout(function () { URL.revokeObjectURL(url); }, 10000);
  }
});
