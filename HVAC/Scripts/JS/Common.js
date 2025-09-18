var _decimal = $('#hdncompanydecimal').val();
var _numberformat = $('#hdnnumberformat').val();
function numberWithCommas(nStr) {
    //alert(nStr);

   

    var _value = '';

    if (nStr != 'undefined' && nStr != null && nStr!='NaN') {
        _value = nStr.toString().replaceAll(',', '');
    }
    if (nStr == 'NaN' ||nStr=='0' || nStr=='0.00' || nStr=='')
        _value = 0;
    _value = parseFloat(_value).toFixed(_decimal);
    //return _value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    if (_numberformat == "Lakhs")
        resultstr = getnumberformatLakhs(_value);
    else
        resultstr = addCommas(_value);

    return resultstr;

    //_value += '';
    //var x = _value.split('.');
    //var x1 = x[0];
    //var x2 = x.length > 1 ? '.' + x[1] : '';
    //var rgx = /(\d+)(\d{3})/;
    //while (rgx.test(x1)) {
    //    x1 = x1.replace(rgx, '$1' + ',' + '$2');
    //}
    //return x1 + x2;
}

function parsenumeric(obj) {
    console.log($(obj).attr('id'));
    if ($(obj).val() == 'undefined' || $(obj).val() == undefined)
        return 0;

    if ($(obj).val().toString().trim() == '') {
        return 0;
    }
    else {
        var _value = $(obj).val();
        if (_value == 'NaN')
            return 0;
        if (_value != undefined && _value != 'undefined') {
            _value = _value.replaceAll(',', '');
            return parseFloat(parseFloat(_value).toFixed(_decimal));
        }
        else {
            return 0;
        }
    }
}

//millions format
function addCommas(nStr) {

    nStr += '';
    var x = nStr.split('.');
    var x1 = x[0];
    var x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}

function getnumberformatLakhs(nStr) {
    var x = nStr.split('.');
    var x1 = x[0];
    var x2 = x.length > 1 ? '.' + x[1] : '';
    var lastThree = x1.substring(x1.length - 3);
    var otherNumbers = x1.substring(0, x1.length - 3);
    if (otherNumbers != '')
        lastThree = ',' + lastThree;
    var res = otherNumbers.replace(/\B(?=(\d{2})+(?!\d))/g, ",") + lastThree + x2;
    return res;
}
