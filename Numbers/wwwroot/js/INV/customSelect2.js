var ids = [];
var idsDep = [];
function _applySelect(item, self, value) {
    setTimeout(function () {
        item.select2({
            width: "100%", //self.width,
            placeholder: 'Search for an Item',
            //allowClear: true,
            templateResult: formatOption,
            ajax: ({
                url: '/AR/Api/GetItems',
                dataType: 'json',
                delay: 250,
                processResults: function (data, params) {
                    return {
                        results: data
                    };
                },
                cache: true
            })
        });
    });
}
function formatOption(data) {
    var option = $('<div><strong>Code : </strong>' + data.code + '</div><div><strong>Name : </strong>' + data.text + '</div><div><strong>Category : </strong>' + data.category + '</div>');
    return option;
}

function _applySelectInv(item, self, value) {
    setTimeout(function () {
        item.select2({
            width: "100%", //self.width,
            placeholder: 'Search for an Item',
            //allowClear: true,
            templateResult: formatOptionInv,
            ajax: ({
                url: '/AR/Api/GetInvItems',
                dataType: 'json',
                delay: 250,
                processResults: function (data, params) {
                    return {
                        results: data
                    };
                },
                cache: true
            })
        });
    });
}
function formatOptionInv(data) {
    var option = $('<div>' + data.text + '</div>');
    return option;
}
