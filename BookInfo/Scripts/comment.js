var modalCommentBodyId = "#modal_comment_body";
var noteid = -1;
//sayfa yüklendikten sonra..
$(function () {
    $('#modal_comment').on('show.bs.modal', function (e) {
        var btn = $(e.relatedTarget);
        noteid = btn.data("note-id");
        $(modalCommentBodyId).load("/Comment/ShowNoteComments/" + noteid);
    })

});
function doComment(btn, e, commentid, spanid) {
    var button = $(btn);
    var mode = button.data("edit-mode");
    if (e == "edit_clicked") {

        if (!mode) {
            button.data("edit-mode", true);//data attr, bu şekilde atama yapabiliriz
            button.removeClass("btn-warning");
            button.addClass("btn-success");
            var btnSpan = button.find("span");
            btnSpan.removeClass("glyphicon-edit");
            btnSpan.addClass("glyphicon-ok");
            $(spanid).addClass("editable");
            $(spanid).attr("contenteditable", true);
            $(spanid).focus();
        }
        else {
            button.data("edit-mode", false);//data attr, bu şekilde atama yapabiliriz
            button.addClass("btn-warning");
            button.removeClass("btn-success");
            var btnSpan = button.find("span");
            btnSpan.addClass("glyphicon-edit");
            btnSpan.removeClass("glyphicon-ok");
            $(spanid).removeClass("editable");
            $(spanid).attr("contenteditable", false);
            var txt = $(spanid).text();
            $.ajax({
                method: "POST",
                url: "/Comment/Edit/" + commentid,
                data: { text: txt }
            }).done(function (data) {
                if (data.result) {
                    //yorumlar partial tekrar yüklenir.
                    $('#modal_comment_body').load("/Comment/ShowNoteComments/" + noteid);
                }
                else {
                    alert("Yorum güncellenemedi.");
                }

            }).fail(function () {
                alert("Sunucu ile bağlantı kurulamadı");

            });
        }

    }
    else if (e == "delete_clicked") {
        var dialog_res = confirm("Yorum silinsin mi?");
        if (!dialog_res) return false;

        $.ajax({
            method: "GET",
            url: "/Comment/Delete/" + commentid

        }).done(function (data) {
            if (data.result) {
                //yorumlar partial tekrar yüklenir.
                $('#modal_comment_body').load("/Comment/ShowNoteComments/" + noteid);
            } else {
                alert("Yorum silinemedi");
            }
        }).fail(function () {
            alert("Sunucu ile bağlantı kurulamadı");
        })
    }

    else if (e == "new_clicked") {
        var txt = $("#new_comment_text").val(); //Create action'ında almış olduğumuz Comment tipindeki comment nesnesinde text propertysi olduğu için data'daki text'e direkt ilgili yorumu yazacaktır.
        $.ajax({
            method: "POST",
            url: "/Comment/Create",
            data: { "text": txt, "noteid": noteId }
        }).done(function (data) {
            if (data.result) {
                $('#modal_comment_body').load("/Comment/ShowNoteComments/" + noteid);
            }
            else {
                alert("Yorum eklenemedi");
            }
        }).fail(function () {
            alert("Sunucu ile bağlantı kurulamadı")
        })
    }

}