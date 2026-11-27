using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

//Recordatorios de repaso sin servidor: crea un evento de calendario (.ics) a la hora del siguiente repaso.
//En el móvil (versión web) Safari lo abre, se añade al Calendario y el Calendario avisa a esa hora.
public static class Notifications
{
    private const string EventTitle = "BatxiTest: toca repasar"; //Título del evento en el calendario
    private const string EventDescription = "Ya puedes hacer el siguiente repaso de: {0}"; //{0} = título del test
    private const int EventMinutes = 15; //Lo que dura el evento en el calendario

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void DownloadCalendarFile(string fileName, string content); //Plugins/WebGL/Notifications.jslib
#endif

    //Recordatorio para cuando se pueda volver a hacer este test. Si ya se puede, no hay nada que recordar
    public static void RemindNextRepetition(QuizData quiz)
    {
        long seconds = QuizProgress.SecondsUntilSpaced(quiz);
        if (seconds <= 0) return;

        DateTime start = DateTime.UtcNow.AddSeconds(seconds);
        string calendar = BuildEvent(start, string.Format(EventDescription, quiz.title));
        string fileName = "repaso.ics";

#if UNITY_WEBGL && !UNITY_EDITOR
        DownloadCalendarFile(fileName, calendar);
#else
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(path, calendar);
        Debug.Log("Recordatorio guardado en: " + path);
#endif
    }

    //Formato iCalendar: lo entienden el Calendario del iPhone, Google Calendar y Outlook. Las líneas van con \r\n
    private static string BuildEvent(DateTime startUtc, string description)
    {
        StringBuilder ics = new StringBuilder();
        ics.Append("BEGIN:VCALENDAR\r\n");
        ics.Append("VERSION:2.0\r\n");
        ics.Append("PRODID:-//BatxiTest//Recordatorios//ES\r\n");
        ics.Append("BEGIN:VEVENT\r\n");
        ics.Append("UID:" + Guid.NewGuid() + "@batxitest\r\n");
        ics.Append("DTSTAMP:" + IcsTime(DateTime.UtcNow) + "\r\n");
        ics.Append("DTSTART:" + IcsTime(startUtc) + "\r\n");
        ics.Append("DTEND:" + IcsTime(startUtc.AddMinutes(EventMinutes)) + "\r\n");
        ics.Append("SUMMARY:" + Escape(EventTitle) + "\r\n");
        ics.Append("DESCRIPTION:" + Escape(description) + "\r\n");
        ics.Append("BEGIN:VALARM\r\n"); //El aviso, justo a la hora del evento
        ics.Append("ACTION:DISPLAY\r\n");
        ics.Append("DESCRIPTION:" + Escape(EventTitle) + "\r\n");
        ics.Append("TRIGGER:PT0M\r\n");
        ics.Append("END:VALARM\r\n");
        ics.Append("END:VEVENT\r\n");
        ics.Append("END:VCALENDAR\r\n");
        return ics.ToString();
    }

    private static string IcsTime(DateTime utc)
    {
        return utc.ToString("yyyyMMdd'T'HHmmss'Z'");
    }

    private static string Escape(string text)
    {
        return text.Replace("\\", "\\\\").Replace(";", "\\;").Replace(",", "\\,").Replace("\n", "\\n");
    }
}
