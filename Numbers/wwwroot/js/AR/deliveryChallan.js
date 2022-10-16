var ids = [];
$('.remove-item').click(function () {
    var element = this.id;
    var id = element.replace("remove-item", "");
    //ids to remove
    var itemId = $('#PRItemId' + id).val();
    if (itemId != null) {
        ids.push(itemId);
    }
    // console.log(totalAmount);
    $('#IdsDeleted').text(ids);
    $("#sales-Order" + id).remove();
    //var amount = $('#Amount' + id).val();

});
$('.calculate-total').on('keyup', function () {
    var elementId = this.id;
    //var id = elementId.slice(elementId.length - 1);
    var id = elementId.split('_');
    id = id[1];
});
function getItemDetails(id) {
    var element = id.replace("SaleOrderId", "");
    var value = $('#' + id).val();
    $.ajax({
        type: 'GET',
        async: false,
        url: '/AR/Api/GetOrder?id=' + value,
        contentType: 'JSON'
    }).done(function (data) {
        $('#ExpiryDate' + element).val(data.saleOrderDate);
        $('#SaleOrderBalance' + element).val(data.total);
        $('#Bonus' + element).val(data.total);
        $('#DCBalance' + element).val(data.total);
        $('#StockInStore' + element).val(data.saleOrderNo);
        $('#CompanyId' + element).val(data.companyId);
    });
}