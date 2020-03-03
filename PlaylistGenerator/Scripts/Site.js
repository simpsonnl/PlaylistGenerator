
$(document).ready(function () {
    
    $('#login').click(function () {
        location.href = "https://playlistgenerator20200223044833.azurewebsites.net/Authorize/Login/";
    });


    $('#showhidetarget').hide();

    $('#showhidetrigger').click(function () {
        $('#showhidetarget').toggle(400);
    });

    $('#recentArtists').hide();

    var url = "/Home/SearchTrack";
    var boxId = "#searchTrack_Name";
    var count = 1;
    var boxVal = "";

    $(boxId).autocomplete({
        source: function (request, response) {
            $.ajax({
                url: url,
                type: "POST",
                dataType: "json",
                data: { query: request.term },
                success: function (data) {
                    response($.map(data, function (item) {
                        return { label: item.Name, value: item.Id };

                    }))

                }
            })
        },

        focus: function (event, ui) {
            var v = ui.item.label;
            //$(boxId).attr('placeholder', v);
            event.preventDefault();
        },
        select: function (event, ui) {
            var v = ui.item.label;
            $('#inputId').val(ui.item.value);
            $(boxId).val(v);

            // update what is displayed in the textbox
            $(this).value = v;
            $('#submit').attr('disabled', false);
            count = 0;
            boxVal = v;
            //this.form.submit();
            return false;
        }
    });

    $(boxId).on('keyup keypress', function (e) {
        var keyCode = e.keyCode || e.which;
        var nowBoxVal = $(boxId).val();
        if (keyCode === 13) {
            return;
        }
        if (boxVal != nowBoxVal) {
            $('#submit').attr('disabled', true);
        } else {
            if (nowBoxVal != "") {
                $('#submit').attr('disabled', false);
            }
        }


    });

    $('input:checkbox').change(function () {
        if (document.getElementById('toggle-two').checked) {

            $("#searchTrack_Name").attr('placeholder', 'Search for an artist...');
            $("#searchTrack_Name").attr('id', 'searchArtist_Name');
            $('#recentTracks').hide();
            $('#recentArtists').show();
            $('#imageSelectLabel').html('Or select one of your recent artists.');
            $("#isTrack").val(false);
            url = "/Home/SearchArtistTerm";
            boxId = "#searchArtist_Name";

        } else {
            $("#searchArtist_Name").attr('placeholder', 'Search for a song...');
            $("#searchArtist_Name").attr('id', 'searchTrack_Name');
            $('#recentTracks').show();
            $('#recentArtists').hide();
            $('#imageSelectLabel').html('Or select one of your recent songs.');
            $("#isTrack").val(true);
            url = "/Home/SearchTrack";
            boxId = "#searchTrack_Name";
        }
    });

    $('#numberOfTracks').change(function () {
        var v = $('#numberOfTracks').val();
        var x = document.getElementsByClassName("numTracks");
        for (var i = 0; i < x.length; i++) {
            x[i].value = v;
        }
    });

    //function for the slider value
    $(document).on('input', '#range', function () {
        var v = $('#range').val();
        var x = document.getElementsByClassName("range");
        for (var i = 0; i < x.length; i++) {
            x[i].value = v;
        }

        var NumberOfTracksVal = $('#numberOfTracks').val();
        $('#rangeLabel').html($(this).val());
        $('#recentArtistsArtistrange').val($(this).val());
        if ($(this).val() == 1) {

            $('#rangeLabel').html("A couple artists");
            $('#100Option').attr('disabled', true);
            $('#50Option').attr('disabled', true);
            if (NumberOfTracksVal != 30) {
                ($('#numberOfTracks').val(30));
            }
        } else if ($(this).val() == 2) {
            $('#rangeLabel').html("A few artists");
            $('#100Option').attr('disabled', true);
            $('#50Option').attr('disabled', false);
            if (NumberOfTracksVal == 100) {
                ($('#numberOfTracks').val(50));
            }


        } else if ($(this).val() == 3) {
            $('#rangeLabel').html("A good amount of artists");

        } else if ($(this).val() == 4) {
            $('#rangeLabel').html("Now were talking");
        } else {
            $('#rangeLabel').html("Gimme all of em");
            $('#100Option').attr('disabled', false);
            $('#50Option').attr('disabled', false);
        }
    });

    $('#createForm').on('keyup keypress', function (e) {
        var keyCode = e.keyCode || e.which;
        if (keyCode === 13) {
            e.preventDefault();
            return false;
        }
    });


    $('#submit').click(function () {

        $('#page-loading').removeAttr('hidden');
    });

    $('#save-button').click(function () {

        $('#page-loading').removeAttr('hidden');
    });
});