function timedCount() {
    if (comm_seconds == 0) {
        $(comm_obj).prop('disabled', false);
        if ($(comm_obj).prop('localName') == 'a' || $(comm_obj).prop('localName') == 'button')
            $(comm_obj).text(comm_text);
        else
            $(comm_obj).val(comm_text);
    }
    else {
        if ($(comm_obj).prop('localName') == 'a' || $(comm_obj).prop('localName') == 'button')
            $(comm_obj).text('請等待(' + comm_seconds + ')');
        else
            $(comm_obj).val('請等待(' + comm_seconds + ')');
        comm_seconds--;
        t = setTimeout(timedCount, 1000)
    }
}
function timedSet(obj, show_text, Seconds) {
    comm_obj = obj;
    comm_text = show_text;
    comm_seconds = Seconds || 5;
    timedCount();
}