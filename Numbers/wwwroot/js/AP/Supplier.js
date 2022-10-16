function ValidWebsite() {
    debugger
    var input = $('#webValue').val();
    if (input.substring(0, 4) == 'www.') { input('http://www.' + input.substring(4)); }
    var re = /(http|https):\/\/[\w-]+(\.[\w-]+)+([\w.,@?^=%&:\/~+#-]*[\w@?^=%&\/~+#-])?/;
    var is_url = re.test(input);
    if (!is_url) {
        swal("", "Please Enter Valid Address", "warning")
        $('#webValue').val("")
        return false;
    }
}