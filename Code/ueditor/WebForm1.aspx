<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="ueditor.WebForm1"  validateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>完整demo</title>
    <meta http-equiv="Content-Type" content="text/html;charset=utf-8" />
    <script type="text/javascript" charset="utf-8" src="ueditor.config.js"></script>
    <script type="text/javascript" charset="utf-8" src="ueditor.all.min.js"> </script>
    <!--建议手动加在语言，避免在ie下有时因为加载语言失败导致编辑器加载失败-->
    <!--这里加载的语言文件会覆盖你在配置项目里添加的语言类型，比如你在配置项目里配置的是英文，这里加载的中文，那最后就是中文-->
    <script type="text/javascript" charset="utf-8" src="lang/zh-cn/zh-cn.js"></script>
    <script src="jquery.min.js"></script>
</head>
<body>
    <div>
        <h1>完整demo</h1>
        <script id="editor" type="text/plain" style="width: 1024px; height: 500px;"></script>
        <form method="post"  runat="server">

            <input type="text" name="a" id="a" runat="server" />
            <input type="hidden" name="editorValue" id="editorValue" />
            <input type="submit" value="提交" onclick="getLocalData()" />
            <button onclick="setContent(true)">追加内容</button>

        </form>
    </div>
    <script type="text/javascript">

        //实例化编辑器
        //建议使用工厂方法getEditor创建和引用编辑器实例，如果在某个闭包下引用该编辑器，直接调用UE.getEditor('editor')就能拿到相关的实例
        var ue = UE.getEditor('editor');

        function getLocalData() {
            //alert(UE.getEditor('editor').execCommand("getlocaldata"));
            document.getElementById("editorValue").value = UE.getEditor('editor').execCommand("getlocaldata");
        }
        setTimeout(function () {
            var htm = $("#a").val();
            UE.getEditor('editor').setContent(htm, false);
        }, 500);
        
        function setContent(isAppendTo) {
            var arr = [];
            arr.push("使用editor.setContent('欢迎使用ueditor')方法可以设置编辑器的内容");
            UE.getEditor('editor').setContent('欢迎使用ueditor', isAppendTo);
            alert(arr.join("\n"));
        }
    </script>
</body>
</html>
