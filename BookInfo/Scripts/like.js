//sayfa tamamen yuklendikten sonra
$(function () {

    var noteids = [];

    //div olup içinde data-note-id attribute'u olanların hepsini getir ve each ile hepsinde dön
    $("div[data-note-id]").each(function (i, e) {
        noteids.push($(e).data("note-id"));

    });
    // console.log(noteids);
    $.ajax({
        method: "POST",
        url: "/Book/GetLiked",
        data: { ids: noteids }
    }).done(function (data) {
        // console.log(data);
        if (data.result != null && data.result.length > 0) {
            for (var i = 0; i < data.result.length; i++) {
                var id = data.result[i];
                //div olup içinde data-note-id attr'si olanın id'si, bir üstteki id değişkeine eşit olan...
                var likedNote = $("div[data-note-id]=" + id + "]");
                //bulduktan sonra içindeki butonu bulalım...
                var btn = likedNote.find("btn[data-liked]");
                var span = btn.find("span.like-star");
                //btn'un liked attr'sini true'ya cektik
                btn.data("liked", true);
                span.removeClass("glyphicon glyphicon-star-empty");
                span.addClass("glyphicon-star");
            }
        }
    }).fail(function () {

    });

    // console.log(noteids);

    $("btn[data-liked]").click(function () {
        var btn = $(this);
        var liked = btn.data("liked");
        var noteid = btn.data("note-id");
        var spanStar = btn.find("span.like-star");
        var spanCount = btn.find("span.like-count");

        $.ajax({
            method: "POST",
            url: "/Book/SetLikeState",
            data: { "noteid": noteid, "liked": !liked }//false ise true, true ise false yapmak istediğimiz için tersini gönderiyoruz

        }).done(function (data) {

            // console.log(data);
            if (data.hasError) {
                alert(data.errorMessage);
            }
            else {//like'lama işlemi başarılıysa
                liked = !liked;
                btn.data("liked", liked);
                spanCount.text(data.result);

                spanStar.removeClass("glyphicon-star-empty");
                spanStar.removeClass("glyphicon-star");

                if (liked) {
                    spanStar.addClass("glyphicon-star");
                }
                else {
                    spanStar.addClass("glyphicon-star-empty");
                }
            }

        }).fail(function () {
            alert("Sunucu ile bağlantı kurulamadı");
        });
    });
});