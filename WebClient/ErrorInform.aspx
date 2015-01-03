<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ErrorInform.aspx.cs" Inherits="WebClient.ErrorInform" Theme="GlobalSkin"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Ошибка</title>
</head>
<body>
    <form id="form1" runat="server">
    <div id="blockerrorinform">
        <h6 id="errorheader">Ошибка</h6>
        <br />
        Извините, но произошла ошибка.
        <br />
        <br />
        Возможная причина:
        <br />
        <asp:TextBox ID="ExceptionMessage" runat="server" ReadOnly="true" TextMode="MultiLine" Width="100%"/>
        <br />
        <br /> 
        Возможно вы ввели не корректные данные
        или не имеете прав на выполнение запрошенной операции. Так же причиной может быть
        отсутсвие подключения к удаленному серверу приложений.
        <br /> 
        Попробуйте отлогиниться, очистить
        кеш и куки браузера, а затем повторить операцию. В случае если ошибка повториться, 
        сообщите системному администратору.
        <br /> 
        Ниже вы видите описание ошибки, скопируйте эти данные
        и предоставьте их системному администратору, данные сведения могут заметно ускорить
        процесс устранения неполадок.
        <br /> 
        Спасибо.
        <br />
        <br />
        Служебная информация:
        <br />
        <asp:TextBox ID="ExceptionDetails" runat="server" ReadOnly="true" TextMode="MultiLine" Width="100%"/>
    </div>
    </form>
</body>
</html>
