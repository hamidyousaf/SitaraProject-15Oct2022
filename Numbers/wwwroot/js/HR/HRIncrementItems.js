var ids = [];
//ids to remove
$('.remove-item').click(function () {
    var element = this.id;
    var id = element.replace("remove-item", "");
    //ids to remove
    var itemId = $('#IncrementItemId' + id).val();
    if (itemId != null) {
        ids.push(itemId);
    }
    $('#IdsDeleted').text(ids);
    $("#increment" + id).remove();
});