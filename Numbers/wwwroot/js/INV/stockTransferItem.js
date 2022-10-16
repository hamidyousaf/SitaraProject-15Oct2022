//global array declaration
var ids = [];
$('.remove-item').click(function () {
    var element = this.id;
    var id = element.replace("remove-item", "");

    var itemId = $('#StockTransferItemId' + id).val();
    if (itemId != null) {
        ids.push(itemId);
    }
    //for remove Id of Adjustmentitems
    $('#IdsDeleted').text(ids);
    $("#current-item" + id).remove();
});
function getItemValues(itemId, val) {
    var id = itemId.replace("ItemId", "");
    var elementItem = $('#' + itemId).val();
    $.ajax({
        url: '/AR/Invoice/GetUOM?id=' + val,
        type: 'GET'
    }).done(function (data) {
        $('#UOMId_' + id).val(data[0].id);
        $('#UOM_Value' + id).val(data[0].uom);
    });
    //calling stock from this function
    var wareHouseToId = $('#WareHouseToId').val();
    var transferDate = $('#TransferDate').val();
    var stock = getStock(elementItem, wareHouseToId, transferDate);
    stock.done(function (data) {
        $('#Stock_' + id).val(data.toFixed(2));
    });
}

// qty < stock validation
function StockTransferForm() {
    var i = true;
    $('input[name="Qty"]').each(function () {
        var split = (this.id).split('_');
        var id = split[1];
        var stock = parseFloat($('#Stock_' + id).val());
        var qty = parseFloat($('#Qty_' + id).val());
        if (qty > stock) {
            alert("'Qty' should be lower than 'stock'.");
            i = false;
            return false;
        }
    });
    if (i) $("#TransferFormId").submit();
}




