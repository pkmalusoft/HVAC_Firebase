var _decimal = "2";
function numberWithCommas(x) {
    if (x != null)
        return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    else {
        return '';
    }
}
function getboxno() {
    var maxrow = $('#ItemBody > tr').length;
    if (maxrow == 0) {
        $('#BoxNo').html('01');
    }
    else {
        var totalpcs = 0;
        for (i = 0; i < maxrow; i++) {
            var deleted = $('#hdnItemdeleted_' + i).val();
            if (deleted == 'false' || deleted == '' || deleted == null) {

                var otherqty = 1; //$('#ItemQty_' + i).val();

                totalpcs = parseFloat(totalpcs) + parseFloat(otherqty); //Box qty

            }
        }
        totalpcs = parseFloat(totalpcs) + 1; //Box qty
        if (totalpcs <= 9) {
            $('#BoxNo').html('0' + (totalpcs).toString());
        }
        else {
            $('#BoxNo').html(totalpcs.toString());
        }
    }
}
function bindproductgrid() {
    var idtext = "ProductName_";
    $('[id^=' + idtext + ']').each(function (index, item) {

        
        if ($('#TaxType').val() == 2) { //VAT 
          
            $('#spantaxdetail_' + index).html('VAT ' + parseFloat($('#hdnTaxPercent_' + index).val()).toFixed(2) + '%');
        }
        else if ($('#TaxType').val() == 1) { //VAT {
            if ($('#InvoiceType').val() == 1)//Local
            {
                $('#spantaxdetail_' + index).html('SGST :' + parseFloat($('#hdnSGST_' + index).val()).toFixed(2) + '% CGST :' + parseFloat($('#hdnCGST_' + index).val()).toFixed(2) + '%');
            }
            else if ($('#InvoiceType').val() == 2)//Export
            {
                $('#spantaxdetail_' + index).html('IGST :' + parseFloat($('#hdnIGST_' + index).val()).toFixed(2) + '%');
            }
            else {
                $('#spantaxdetail_' + index).html('');
            }

        }
        //showtax1(item, index);
        $("#ProductName_" + index).autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/Product/GetProductName',
                    datatype: "json",
                    data: {
                        term: request.term, ProductTypeID : $('#ProductTypeID1_' + index).val()
                    },
                    success: function (data) {
                        response($.map(data, function (val, item) {
                            return {
                                label: val.ProductName,
                                value: val.ProductID,
                                UnitId: val.UnitId,
                                SalesRate: val.SalesRate,
                                UnitName: val.UnitName,
                                SGST: val.SGST,
                                CGST: val.CGST,
                                IGST: val.IGST,
                                TaxPercent: val.TaxPercent

                            }
                        }))
                    }
                })
            },
            minLength: 1,
            autoFocus: false,
            focus: function (event, ui) {
                $("#ProductName_"+index).val(ui.item.label);
                $('#ProductID_' +index).val(ui.item.value);
                $('#UnitID_'+index).val(ui.item.UnitId);
                $('#UnitName_'+index).val(ui.item.UnitName);
                $('#SalesRate_' + index).val(numberWithCommas(ui.item.SalesRate));
                $('#ProductID_'+index).val(ui.item.value);
                $('#SGST_'+index).val(ui.item.SGST);
                $('#CGST_'+index).val(ui.item.CGST);
                $('#IGST_'+index).val(ui.item.IGST);
                $('#TaxPercent_'+index).val(ui.item.TaxPercent);
                if ($('#Qty_'+index).val() == '')
                    $('#Qty_'+index).val(1);
                calculateNetvalue1(index);
                showtax1(ui.item,index);
            },
            select: function (e, ui) {
                e.preventDefault();
                $("#ProductName_" + index).val(ui.item.label);
                $('#ProductID_' + index).val(ui.item.value);
                $('#UnitID_' + index).val(ui.item.UnitId);
                $('#UnitName_' + index).val(ui.item.UnitName);
                $('#SalesRate_' + index).val(numberWithCommas(ui.item.SalesRate));
                $('#ProductID_' + index).val(ui.item.value);
                $('#SGST_' + index).val(ui.item.SGST);
                $('#CGST_' + index).val(ui.item.CGST);
                $('#IGST_' + index).val(ui.item.IGST);
                $('#TaxPercent_' + index).val(ui.item.TaxPercent);
                if ($('#Qty_' + index).val() == '')
                    $('#Qty_' + index).val(1);
                calculateNetvalue1(index);
                showtax1(ui.item, index);
            },

        });
    });

}
function setCurrencywiseSalesInput() {
    if ($('#CurrencyID').val() == $('#BranchCurrencyId').val()) {
        var idtext = 'SalesRate_';
        $('[id^=' + idtext + ']').each(function (index, item) {
            $('#SalesRate_' + index).removeAttr('readonly');
            $('#SalesRateFC_' + index).attr('readonly', 'readonly');
            if ($('#SalesRateFC_' + index).val() != '')
                $('#SalesRateFC_' + index).val('');

            if ($('#SalesRateFC_' + index).val() != '')
                $('#SalesRateFC_' + index).val('');


        });
        $('#SalesRateFC').attr('readonly', 'readonly');
        $('#SalesRate').removeAttr('readonly');
        $('#BranchCurrency').val('YES');
        $('#ExRate').attr('readonly', 'readonly');
    }
    else {

        var idtext = 'SalesRate_';
        $('[id^=' + idtext + ']').each(function (index, item) {
            $('#SalesRate_' + index).attr('readonly', 'readonly');
            $('#SalesRateFC_' + index).removeAttr('readonly');
                      

        });
        $('#SalesRate').attr('readonly', 'readonly');
        $('#SalesRateFC').removeAttr('readonly');
        $('#BranchCurrency').val('NO');
        $('#ExRate').removeAttr('readonly');
    }
}

function calculateLocalRate1(index1) {
    var fcRate = parsenumeric($('#SalesRateFC_' + index1));
    var rate = parseFloat(fcRate) * parseFloat($('#ExRate').val());
    $('#SalesRate_' + index1).val(numberWithCommas(parseFloat(rate).toFixed(_decimal)));
    calculateNetvalue1(index1);
}
function calculateLocalRate() {
    var fcRate = parsenumeric($('#SalesRateFC'));
    var rate = parseFloat(fcRate) * parseFloat($('#ExRate').val());
    $('#SalesRate').val(parseFloat(rate).toFixed(_decimal));
    calculateNetvalue();
}
function calculateNetvalue() {
   
    var qty = $('#Qty').val();
    var Rate = parsenumeric($('#SalesRate'));

    if ($('#CurrencyID').val() == $('#BranchCurrencyId').val()) {
        var fcrate = parsenumeric($('#SalesRateFC'));
        if (parseFloat(fcrate)>0)
        {
            $('#SalesRateFC').val('');
        }
    }
    if ($('#DiscountPercent1').val() =='') {
        $('#DiscountPercent1').val(0);
    }
    var Discount = $('#DiscountPercent1').val();
    var Discountamount = 0;
    var totalvalue = 0;
    var discountlocal = 0;
    if (parseFloat(Discount) > 0) {
        if ($('#CurrencyID').val() == $('#BranchCurrencyId').val()) {
            Discountamount = parseFloat((parseFloat(qty) * parseFloat(Rate)) * (Discount / 100.00)).toFixed(_decimal);
            $('#DiscountAmount1').val(numberWithCommas(Discountamount));
            $('#hdnDiscountAmount1').val(numberWithCommas(Discountamount));
            $('#hdnDiscountAmount1FC').val(0);
            discountlocal = Discountamount;
        }
        else {
            var fcrate = parsenumeric($('#SalesRateFC'));
            Discountamount = parseFloat((parseFloat(qty) * parseFloat(fcrate)) * (Discount / 100.00)).toFixed(_decimal);
            
            $('#DiscountAmount1').val(numberWithCommas(Discountamount));
            discountlocal = parseFloat(Discountamount) * parseFloat($('#ExRate').val());
            $('#hdnDiscountAmount1').val(discountlocal);
            $('#hdnDiscountAmount1FC').val(Discountamount);
        }
    }
    else {
        if ($('#DiscountAmount1').val() != null && $('#DiscountAmount1').val() != '') {
            if ($('#CurrencyID').val() == $('#BranchCurrencyId').val()) {
                Discountamount = parsenumeric($('#DiscountAmount1'));
            }
            else {
                Discountamount = parsenumeric($('#DiscountAmount1'));
                discountlocal = parseFloat(Discountamount) * parseFloat($('#ExRate').val());
                $('#hdnDiscountAmount1').val(discountlocal);
                $('#hdnDiscountAmount1FC').val(Discountamount);
            }
            
        }
    }
    
    totalvalue = ((parseFloat(qty) * parseFloat(Rate)) - parseFloat(discountlocal));
    if ($('#TaxType').val() == '1' && $('#InvoiceType').val()=='1' ) {//Local GST {
        if ($('#nSGST').val() == '')
            $('#nSGST').val(0);
        var sgst = $('#nSGST').val();
        var sgstamt = parseFloat(sgst)/100.00 * totalvalue;
        $('#SGSTAmount1').html(numberWithCommas(parseFloat(sgstamt).toFixed(_decimal)));
        $('#nSGSTAmount').val(parseFloat(sgstamt).toFixed(_decimal));

        if ($('#nCGST').val() == '')
            $('#nCGST').val(0);
        var cgst = $('#nCGST').val();
        var cgstamt = parseFloat(cgst) / 100.00 * totalvalue;
        $('#nCGSTAmount').val(parseFloat(cgstamt).toFixed(_decimal));
        $('#CGSTAmount1').html(numberWithCommas(parseFloat(cgstamt).toFixed(_decimal)));
        var netvalue = totalvalue + sgstamt + cgstamt;
        $('#NetValue').val(parseFloat(netvalue).toFixed(_decimal));
        $('#NetValue1').html(numberWithCommas(parseFloat(netvalue).toFixed(_decimal)));
    }
    else if ($('#TaxType').val() == '1' && $('#InvoiceType').val() == '3') {// Export IGST
        //if ($('#nIGST').val() == '')
            $('#nIGST').val(0);
        var igst = $('#nIGST').val();
        var igstamt = 0;// parseFloat(igst) / 100.00 * totalvalue;
        $('#IGSTAmount1').html(numberWithCommas(parseFloat(igstamt).toFixed(_decimal)));
        $('#nIGSTAmount').val(parseFloat(igstamt).toFixed(_decimal));
        $('#SGSTAmount1').html(0);
        $('#CGSTAmount1').html(0);
        var netvalue = totalvalue + igstamt;
        $('#NetValue').val(parseFloat(netvalue).toFixed(_decimal));
        $('#NetValue1').html(numberWithCommas(parseFloat(netvalue).toFixed(_decimal)));
    }
    else if ($('#TaxType').val() == '1' && $('#InvoiceType').val() == '2') {//Inter state GST {
        if ($('#nIGST').val() == '')
            $('#nIGST').val(0);
        var igst = $('#nIGST').val();
        var igstamt = parseFloat(igst) / 100.00 * totalvalue;
        $('#IGSTAmount1').html(numberWithCommas(parseFloat(igstamt).toFixed(_decimal)));
        $('#nIGSTAmount').val(parseFloat(igstamt).toFixed(_decimal));

        var netvalue = totalvalue + igstamt;
        $('#NetValue').val(parseFloat(netvalue).toFixed(_decimal));
        $('#NetValue1').html(numberWithCommas(parseFloat(netvalue).toFixed(_decimal)));
    }
    else { //Branch is in VAT Type
        if ($('#nTaxPercent').val() == '')
            $('#nTaxPercent').val(0);
        var taxper = $('#nTaxPercent').val();
        var taxperamt = parseFloat(taxper)/100.00 * totalvalue;
        $('#TaxAmount1').html(parseFloat(taxperamt).toFixed(_decimal));//span
        $('#nTaxAmount').val(parseFloat(taxperamt).toFixed(_decimal));
        var netvalue = totalvalue + taxperamt;
        $('#NetValue').val(parseFloat(netvalue).toFixed(_decimal));
        $('#NetValue1').html(numberWithCommas(parseFloat(netvalue).toFixed(_decimal)));
    }
   

   
    
}
function recalculatenettotal() {
    var itemcount = $('#details > tbody > tr').length;
    var idtext = 'ProductID_';

    $('[id^=' + idtext + ']').each(function (index2, item) {
        
            calculateLocalRate1(index2)
       
    })
}
function calculateNetvalue1(index1) {
    debugger;
    var discountlocal = 0;
    var qty = $('#Qty_' + index1).val();
    var Rate = parsenumeric($('#SalesRate_' + index1));
    if ($('#Discount_' + index1).val() == '') {
        $('#Discount_' + index1).val(0);
    }
    var Discount = $('#Discount_' + index1).val();
    if (Discount == '')
        Discount = 0;
    var Discountamount = 0;
    if (parseFloat(Discount) > 0) {
        if ($('#CurrencyID').val() == $('#BranchCurrencyId').val()) {
            Discountamount = parseFloat((parseFloat(qty) * parseFloat(Rate)) * (Discount / 100.00)).toFixed(_decimal);
            $('#DiscountAmount_' + index1).val(numberWithCommas(Discountamount));
            $('#hdnDiscountAmount_'+index1).val(Discountamount);
            $('#hdnDiscountAmountFC_' + index1).val(0);
            discountlocal = Discountamount;
        }
        else {
            var fcRate = parsenumeric($('#SalesRateFC_' + index1));
            Discountamount = parseFloat((parseFloat(qty) * parseFloat(fcRate)) * (Discount / 100.00)).toFixed(_decimal);

            $('#DiscountAmount_'+index1).val(numberWithCommas(Discountamount));
            discountlocal = parseFloat(Discountamount) * parseFloat($('#ExRate').val());
            $('#hdnDiscountAmount_' + index1).val(discountlocal);
            $('#hdnDiscountAmountFC_' + index1).val(Discountamount);
        }
    } else {
        if ($('#DiscountAmount_' + index1).val() != null && $('#DiscountAmount_' + index1).val() != '') {
            if ($('#CurrencyID').val() == $('#BranchCurrencyId').val()) {
                Discountamount = parsenumeric($('#DiscountAmount_' + index1));
            }
            else {
                Discountamount = parsenumeric($('#DiscountAmount_' +index1));
                discountlocal = parseFloat(Discountamount) * parseFloat($('#ExRate').val());
                $('#hdnDiscountAmount_' + index1).val(discountlocal);
                $('#hdnDiscountAmountFC_' + index1).val(Discountamount);
            }
          
        }
    }
      

    var totalvalue = ((parseFloat(qty) * parseFloat(Rate)) - parseFloat(Discountamount));
    if ($('#TaxType').val() == '1' && $('#InvoiceType').val()=='1' ){ //GST
        if ($('#hdnSGST_' + index1).val() == '')
            $('#hdnSGST_' + index1).val(0);
        var sgst = $('#hdnSGST_' + index1).val();
        var sgstamt = parseFloat(sgst)/100.00 * totalvalue;
        $('#spanSGSTAmount_' + index1).html(numberWithCommas(parseFloat(sgstamt).toFixed(_decimal)));
        $('#hdnSGSTAmount_' + index1).val(parseFloat(sgstamt).toFixed(_decimal));

        if ($('#hdnCGST_' + index1).val() == '')
            $('#hdnCGST_' + index1).val(0);
        var cgst = $('#hdnCGST_' + index1).val();
        var cgstamt = parseFloat(cgst) / 100.00 * totalvalue;
        $('#hdnIGSTAmount_' + index1).val(parseFloat(0).toFixed(_decimal));
        $('#hdnCGSTAmount_' + index1).val(parseFloat(cgstamt).toFixed(_decimal));
        $('#spanCGSTAmount_' + index1).html(numberWithCommas(parseFloat(cgstamt).toFixed(_decimal)));
        
        var netvalue = totalvalue + sgstamt + cgstamt;
        $('#NetValue_' + index1).val(parseFloat(netvalue).toFixed(_decimal));
        $('#spanNetValue_' + index1).html(numberWithCommas(parseFloat(netvalue).toFixed(_decimal)));
    }
    else if ($('#TaxType').val() == '1' && $('#InvoiceType').val() == '2') {//inter state GST {
        if ($('#hdnIGST_' + index1).val() == '')
            $('#hdnIGST_' + index1).val(0);
        var igst = $('#hdnIGST_' + index1).val();
        var igstamt = parseFloat(igst) / 100.00 * totalvalue;
        $('#spanIGSTAmount_' + index1).html(numberWithCommas(parseFloat(igstamt).toFixed(_decimal)));
        $('#hdnIGSTAmount_' + index1).val(parseFloat(igstamt).toFixed(_decimal));
        $('#hdnSGSTAmount_' + index1).val(parseFloat(0).toFixed(_decimal));
        $('#hdnCGSTAmount_' + index1).val(parseFloat(0).toFixed(_decimal));
        var netvalue = totalvalue + igstamt;
        $('#NetValue_' + index1).val(parseFloat(netvalue).toFixed(_decimal));
        $('#spanNetValue_' + index1).html(numberWithCommas(parseFloat(netvalue).toFixed(_decimal)));
    }
    else if ($('#TaxType').val() == '1' && $('#InvoiceType').val() == '3') {//Export GST {
        //if ($('#hdnIGST_' + index1).val() == '')
        $('#hdnIGST_' + index1).val(0);
        $('#hdnSGST_' + index1).val(0);
        $('#hdnCGST_' + index1).val(0);
        var igst = $('#hdnIGST_' + index1).val();
        var igstamt = 0; // parseFloat(igst) / 100.00 * totalvalue;
        $('#spanIGSTAmount_' + index1).html(numberWithCommas(parseFloat(igstamt).toFixed(_decimal)));
        $('#hdnIGSTAmount_' + index1).val(parseFloat(igstamt).toFixed(_decimal));
        $('#hdnSGSTAmount_' + index1).val(parseFloat(0).toFixed(_decimal));
        $('#hdnCGSTAmount_' + index1).val(parseFloat(0).toFixed(_decimal));
        var netvalue = totalvalue + igstamt;
        $('#NetValue_' + index1).val(parseFloat(netvalue).toFixed(_decimal));
        $('#spanNetValue_' + index1).html(numberWithCommas(parseFloat(netvalue).toFixed(_decimal)));
    }
    else {
        if ($('#hdnTaxPercent_' + index1).val() == '')
            $('#hdnTaxPercent_' + index1).val(0);
        
        var taxper = $('#hdnTaxPercent_' + index1).val();
        var taxperamt = parseFloat(taxper)/100.00 * totalvalue;
        $('#hdnTaxAmount_' + index1).val(parseFloat(taxperamt).toFixed(_decimal));
        var netvalue = totalvalue + taxperamt;
        $('#NetValue_' + index1).val(parseFloat(netvalue).toFixed(_decimal));
        $('#spanNetValue_' + index1).html(numberWithCommas(parseFloat(netvalue).toFixed(_decimal)));
    }
    calculatenettotal();
}
function calculatenettotal() {
    var idtext = 'NetValue_';
    var nettotal = 0;
    var sgsttotal = 0;
    var cgsttotal = 0;
    var igsttotal = 0;
    var taxtotal = 0;
    var discounttotal = 0;
    var discountfctotal = 0;
    var nettotalfc = 0;

    $('#spandiscount').html(numberWithCommas(parseFloat(discounttotal).toFixed(_decimal)));

    $('#spansgst').html(numberWithCommas(parseFloat(sgsttotal).toFixed(_decimal)));
    $('#spancgst').html(numberWithCommas(parseFloat(cgsttotal).toFixed(_decimal)));
    $('#spanigst').html(numberWithCommas(parseFloat(igsttotal).toFixed(_decimal)));

    $('#spantax').html(numberWithCommas(parseFloat(taxtotal).toFixed(_decimal)));
    $('#spanTotal').html(numberWithCommas(parseFloat(nettotal).toFixed(_decimal)));
    $('#spanTotalFC').html(numberWithCommas(parseFloat(nettotalfc).toFixed(_decimal)));
    $('#InvoiceTotal').val(parseFloat(nettotal).toFixed(_decimal));
    $('#DiscountAmount').val(parseFloat(discounttotal).toFixed(_decimal));
    $('#DiscountAmountFC').val(parseFloat(discountfctotal).toFixed(_decimal));
    $('#SGSTAmount').val(parseFloat(sgsttotal).toFixed(_decimal));
    $('#CGSTAmount').val(parseFloat(cgsttotal).toFixed(_decimal));
    $('#IGSTAmount').val(parseFloat(igsttotal).toFixed(_decimal));
    $('#TaxAmount').val(parseFloat(taxtotal).toFixed(_decimal));
    $('[id^=' + idtext + ']').each(function (index, item) {
        var deleted = $('#hdnItemdeleted_' + index).val();
        if (deleted != 'true') {
            var netvalue = $('#NetValue_' + index).val();
            var netvaluefc = parsenumeric($('#SalesRateFC_' + index)) + parsenumeric($('#hdnDiscountAmountFC_' + index));
            nettotalfc = parseFloat(nettotalfc) + parseFloat(netvaluefc);
            nettotal = parseFloat(nettotal) + parseFloat(netvalue);

            var sgst = $('#hdnSGSTAmount_' + index).val();

            var cgst = $('#hdnCGSTAmount_' + index).val();
            var igst = $('#hdnIGSTAmount_' + index).val();
            var tax = $('#hdnTaxAmount_' + index).val();
            var discount = parsenumeric($('#hdnDiscountAmount_' + index));
            var discountfc = parsenumeric($('#hdnDiscountAmountFC_' + index));
            if (sgst == '')
                sgst = 0;

            if (cgst == '')
                cgst = 0;

            if (igst == '')
                igst = 0;

            if (tax == '')
                tax = 0;

            if (discount == '')
                discount = 0;
            sgsttotal = parseFloat(sgsttotal) + parseFloat(sgst);
            cgsttotal = parseFloat(cgsttotal) + parseFloat(cgst);
            igsttotal = parseFloat(igsttotal) + parseFloat(igst);
            taxtotal = parseFloat(taxtotal) + parseFloat(tax);
            discounttotal = parseFloat(discounttotal) + parseFloat(discount);
            discountfctotal = parseFloat(discountfctotal) + parseFloat(discountfc);
            if (parseFloat(discountfctotal) > 0) {
                $('#spandiscount').html(numberWithCommas(parseFloat(discountfctotal).toFixed(_decimal)));
            }
            else {
                $('#spandiscount').html(numberWithCommas(parseFloat(discounttotal).toFixed(_decimal)));
            }
            

            $('#spansgst').html(numberWithCommas(parseFloat(sgsttotal).toFixed(_decimal)));
            $('#spancgst').html(numberWithCommas(parseFloat(cgsttotal).toFixed(_decimal)));
            $('#spanigst').html(numberWithCommas(parseFloat(igsttotal).toFixed(_decimal)));
            
            $('#spantax').html(numberWithCommas(parseFloat(taxtotal).toFixed(_decimal)));
            $('#spanTotal').html(numberWithCommas(parseFloat(nettotal).toFixed(_decimal)));
            $('#spanTotalFC').html(numberWithCommas(parseFloat(nettotalfc).toFixed(_decimal)));
            $('#InvoiceTotal').val(parseFloat(nettotal).toFixed(_decimal));
            $('#DiscountAmount').val(parseFloat(discounttotal).toFixed(_decimal));
            $('#DiscountAmountFC').val(parseFloat(discountfctotal).toFixed(_decimal));
            $('#SGSTAmount').val(parseFloat(sgsttotal).toFixed(_decimal));
            $('#CGSTAmount').val(parseFloat(cgsttotal).toFixed(_decimal));
            $('#IGSTAmount').val(parseFloat(igsttotal).toFixed(_decimal));
            $('#TaxAmount').val(parseFloat(taxtotal).toFixed(_decimal));

            
        }
    });
}

function getaddress(obj) {
     
    var _address = '';
    if (obj.Address1 != null && obj.Address1 != '')
        _address = obj.Address1;;

    if (obj.Address2 != null && obj.Address2 != '') {
        if (_address != '') {
            _address = _address + "\r" +  obj.Address2 + "\r";
        }
        else {
            _address = obj.Address2;
        }
    }

    if (obj.Address3 != null && obj.Address3 != '') {
        if (_address != '') {
            _address = _address + "\r" + obj.Address3 + "\r";
        }
        else {
            _address = obj.Address3;
        }
    }

    if (obj.CityName != null && obj.CityName != '') {
        if (_address != '') {
            _address = _address + "\r" + obj.CityName + "\r";
        }
        else {
            _address = obj.CityName;
        }
    }
    if (obj.CountryName != null && obj.CountryName != '') {
        if (_address != '') {
            _address = _address + "\r" + obj.CountryName + "\r";
        }
        else {
            _address = obj.CountryName;
        }
    }

    return _address;

}

function showtax(obj) {
    if ($('#TaxType').val() == '2') { //VAT
        $('#spantaxdetail').html('VAT ' + obj.TaxPercent + '%');
    }
    else {
        if ($('#InvoiceType').val() == 1)//Local
        {
            $('#spantaxdetail').html('SGST :' + obj.SGST + '%' + '  CGST :' + obj.CGST + '%');
        }
        else if ($('#InvoiceType').val() == 2)// inter state
        {
            //$('#spantaxdetail').html('SGST :' + obj.SGST + '%' + '  CGST :' + obj.CGST + '%');
            $('#spantaxdetail').html('IGST :' + obj.IGST + '%');
        }
        else {
            $('#spantaxdetail').html('');
        }
    }
}

function showtax1(obj,i) {
    if ($('#TaxType').val() == '2') { //VAT 
        $('#spantaxdetail_'+i).html('VAT ' + obj.TaxPercent + '%');
    }
    else {
        if ($('#InvoiceType').val() == 1)//Local
        {
            $('#spantaxdetail_' + i).html('SGST :' + obj.SGST + '% CGST :' + obj.CGST + '%');
        }
        else if ($('#InvoiceType').val() == 2)//Export
        {
            $('#spantaxdetail_' + i).html('IGST :' + obj.IGST + '%');
        }
        else {
            $('#spantaxdetail_' + i).html('');
        }
      
    }
}

function parsenumeric(obj) {
    if ($(obj).val() == '') {
        return 0;
    }
    else {
        var _value = $(obj).val();
        
        if (_value != undefined) {
            _value = _value.replaceAll(',', '');
            return parseFloat(parseFloat(_value).toFixed(_decimal));
        }
        else {
            return 0;
        }
    }
}

function settaxtype() {

    if ($('#TaxType').val() == 1) { //GST
        if ($('#InvoiceType').val() == 1) {//Local


            var idtext = 'taxlocal';

            $('[class^=' + idtext + ']').each(function (index, item) {
                $(item).removeClass('d-none');
            });

            idtext = 'taxexport';
            $('[class^=' + idtext + ']').each(function (index, item) {
                $(item).addClass('d-none');
            });

            idtext = 'taxvat';

            $('[class^=' + idtext + ']').each(function (index, item) {
                $(item).addClass('d-none');
            });


        }
        else if ($('#InvoiceType').val() == 2) {//interstate

            var idtext = 'taxlocal';

            $('[class^=' + idtext + ']').each(function (index, item) {
                $(item).addClass('d-none');
            });

            idtext = 'taxexport';
            $('[class^=' + idtext + ']').each(function (index, item) {
                $(item).removeClass('d-none');
            });

            idtext = 'taxvat';

            $('[class^=' + idtext + ']').each(function (index, item) {
                $(item).addClass('d-none');
            });
        }
        else if ($('#InvoiceType').val() == 3)  //interstate)
        {

            idtext = 'taxexport';
            $('[class^=' + idtext + ']').each(function (index, item) {
                $(item).addClass('d-none');
            });
        }

    }
    else {
        var idtext = 'taxlocal';

        $('[class^=' + idtext + ']').each(function (index, item) {
            $(item).addClass('d-none');
        });

        idtext = 'taxexport';
        $('[class^=' + idtext + ']').each(function (index, item) {
            $(item).addClass('d-none');
        });

        idtext = 'taxvat';

        $('[class^=' + idtext + ']').each(function (index, item) {
            $(item).removeClass('d-none');
        });

    }
}

function deleteItemtrans(obj) {
    var id = $(obj).attr('id').split('_')[1];
    $(obj).parent().parent().parent().addClass('hide');
    $('#hdnItemdeleted_' + id).val('true');
       
    calculatenettotal();
    resetboxno();

}
function resetboxno() {

    var maxrow3 = $('#ItemBody > tr').length;
    if (maxrow3 == 0) {
        $('#BoxNo').val('01');
    }
    else {
        var totalpcs = 0;
        for (k = 0; k < maxrow3; k++) {
            var deleted = $('#hdnItemdeleted_' + k).val();
            if (deleted == 'false' || deleted == '' || deleted == null) {



                totalpcs = parseFloat(totalpcs) + 1; //Box qty
                if (totalpcs < 10) {
                    $('#BoxNo_' + k).html('0' + (totalpcs).toString());
                }
                else {
                    $('#BoxNo_' + k).html(totalpcs.toString());
                }
            }
        }


    }
}
function getcustomerdetail() {
   
    $.ajax({
        url: '/CustomerMaster/GetCustomerName',
        datatype: "json",
        data: {
            term: $('#CustomerName').val()
        },
        success: function (data) {
            debugger;
            $('#CustomerID').val(0);
            if (data.length > 0) {

                $.each(data, function (index, val) {
                    debugger;
                    if (val.CustomerName == $('#CustomerName').val()) {
                        $("#CustomerName").val(val.CustomerName);

                        $('#CustomerID').val(val.CustomerID);
                        $('#CustomerPhoneNo').val(val.CustomerPhoneNo);
                        $('#CustomerEmailID').val(val.Email);
                        var address = getaddress(val);
                        $('#CustomerAddress').val(address);
                    }

                })
            }
        }
    })
}
function SaveCustomerInvoice() {
    debugger;
 
    var customerid = 0;
    var maxrow = $('#details > tbody > tr').length;
    var totalcharge = parseFloat($('#InvoiceTotal').val());
    if ($('#PaymentModeId').val()==3 && ($('#CustomerID').val() == '' || $('#CustomerID').val() == 0)) {

        Swal.fire("Save Status", "Select Customer to add Invoice!", "warning");
        $('#btnsave').removeAttr('disabled');
        $('#CustomerName').focus();
        return false;
    }
    else if (maxrow == 0) {
        $('#btnsave').removeAttr('disabled');

        Swal.fire("Save Status", "Select Product to add Invoice!", "warning");
        return false;
    }
    else if (totalcharge == 0) {
        $('#btnsave').removeAttr('disabled');
        Swal.fire("Save Status", "Select Product Details to add Invoice!", "warning");

        return false;
    }
    else if (parsenumeric($('#NetValue')) >  0) {
        $('#btnsave').removeAttr('disabled');
        Swal.fire("Save Status", "Click Save or Clear Product Grid to confirm the product adding!", "warning");

        return false;
    }
    $('#btnsave').attr('disabled', 'disabled');
    var RecPObj = {
        CustomerInvoiceID:$('#CustomerInvoiceID').val(),
        CustomerInvoiceNo :$('#CustomerInvoiceNo').val(),
        InvoiceDate: $('#InvoiceDate').val(),
        AcHeadId: $('#AcHeadId').val(),
        CustomerID: $('#CustomerID').val(),
        CustomerName: $('#CustomerName').val(),
        CustomerInvoiceTax:$('#CustomerInvoiceTax').val(),
        PaymentModeId: $('#PaymentModeId').val(),
        InvoiceType: $('#InvoiceType').val(),
        CurrencyID :$('#CurrencyID').val(),
        ExRate: $('#ExRate').val(),
        FuelAmt :$('#FuelAmt').val(),
        SalesPersonID: $('#SalesPersonID').val(),
        CustomerAddress: $('#CustomerAddress').val(),
        CustomerPhoneNo: $('#CustomerPhoneNo').val(),
        Remarks: $('#Remarks').val(),
        ReferenceNo: $('#ReferenceNo').val(),
        CustomerEmailID: $('#CustomerEmailID').val(),
        SGSTAmount: $('#SGSTAmount').val(),
        CGSTAmount: $('#CGSTAmount').val(),
        IGSTAmount: $('#IGSTAmount').val(),
        TaxAmount: $('#TaxAmount').val(),
        DiscountPercent: $('#DiscountPercent').val(),
        DiscountAmount: $('#DiscountAmount').val(),
        DiscountAmountFC: $('#DiscountAmountFC').val(),
        InvoiceTotal: $('#InvoiceTotal').val(),
        DueDate :$('#DueDate').val()
    }

    if (totalcharge == 0) {

        $.ajax({
            type: "POST",
            url: "/CustomerInvoice/SaveCustomerInvoice",
            datatype: "Json",
            data: { model: RecPObj, Details: '' },
            success: function (response) {
                if (response.status == "OK") {

                    Swal.fire("Save Status", response.message, "success");
                    setTimeout(function () {
                        location.href = '/CustomerInvoice/Index'; // Create?id=0';
                    }, 200)

                }
                else {
                    $('#btnsave').removeAttr('disabled');
                    Swal.fire("Save Status", response.message, "error");
                    return false;
                }
            }
        });

    }
    else {
        var Items = [];

        var itemcount = $('#details > tbody > tr').length;
        var idtext = 'ProductID_';

        $('[id^=' + idtext + ']').each(function (index, item) {
            var deleted = $('#hdnItemdeleted_' + index).val();
            if (deleted != 'true' && deleted != 'True') {
                var producttypeid = $('#ProductTypeID1_' + index).val();
                var productid = $('#ProductID_' + index).val();
                var description = $('#Description_' + index).val();
                var sgst = $('#hdnSGST_' + index).val();
                var sgstamount = $('#hdnSGSTAmount_' + index).val();
                var cgst = $('#hdnCGST_' + index).val();
                var cgstamount = $('#hdnCGSTAmount_' + index).val();
                var igst = $('#hdnIGST_' + index).val();
                var igstamount = $('#hdnIGSTAmount_' + index).val();
                var taxpercent = $('#hdnTaxPercent_' + index).val();
                var taxamount = $('#hdnTaxAmount_' + index).val();
                
                var unitid = $('#hdnUnitID_' + index).val();
                var qty = $('#Qty_' + index).val();
                var SalesRate = parsenumeric($('#SalesRate_' + index));
                var SalesRateFC = parsenumeric($('#SalesRateFC_' + index));
                var Discount = $('#Discount_' + index).val();
                var DiscountAmount = parsenumeric($('#hdnDiscountAmount_' + index));
                var DiscountAmountFC = parsenumeric($('#hdnDiscountAmountFC_' + index));
                var NetValue = parsenumeric($('#NetValue_' + index));
                var DefRID = $('#DEFRID_' + index).val();
                var ForDefRID = $('#ForDEFRID_' + index).val();
              
                var invoicedetaild = 0;// $('#hdnCustomerInvoiceDetailID_' + index).val();
                var data = {
                    CustomerInvoiceDetailID: invoicedetaild,
                    CustomerInvoiceID: $('#CustomerInvoiceID').val(),
                    ProductTypeID: producttypeid,
                    ProductID: productid,
                    Description: description,
                    UnitID: unitid,
                    Quantity: qty,
                    SalesRate: SalesRate,
                    SalesRateFC: SalesRateFC,
                    SGST: sgst,
                    SGSTAmt: sgstamount,
                    CGST: cgst,
                    CGSTAmt: cgstamount,
                    IGST: igst,
                    IGSTAmt: igstamount,
                    TaxPercent: taxpercent,
                    TaxAmount: taxamount,
                    DiscountPercent: Discount,
                    DiscountAmount: DiscountAmount,
                    DiscountAmountFC: DiscountAmountFC,
                    NetValue: NetValue,
                    DEFRID: DefRID,
                    ForDEFRID : ForDefRID

                }

                Items.push(data);

            }
            if ((index+1) == (itemcount)) {
                debugger;
                $.ajax({
                    type: "POST",
                    url: "/CustomerInvoice/SaveCustomerInvoice",
                    datatype: "Json",
                    data: { model: RecPObj, Details: JSON.stringify(Items) },
                    success: function (response) {
                        if (response.status == "OK") {

                            Swal.fire("Save Status", response.message, "success");
                            $('#btnsave').removeAttr('disabled');
                            setTimeout(function () {
                                if ($('#CustomerInvoiceID').val() == 0) {
                                    location.href = '/CustomerInvoice/Index'; //Create?id=0';
                                }
                                else {
                                    location.href = '/CustomerInvoice/Index'; //?id=' + response.CustomerInvoiceID;
                                }
                                
                            }, 200)

                        }
                        else {
                            $('#btnsave').removeAttr('disabled');
                            Swal.fire("Save Status", response.message, "error");
                            return false;
                        }
                    }
                });
            }
        });
    }
   
}

function showitempopup(rowindex) {

    var title = 'Invoice NO.' + $('#hdnInvoiceNo_' + rowindex).val() + '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ' + $('#hdnInvoiceDate_' + rowindex).val();
    $('#h4PackListTitle').html(title);
    $('#ParentRowCount').val(rowindex);

    var itemcount = $('#PackItemBody > tr').length;
    var idtext = 'str_';

    $.ajax({
        type: 'POST',
        url: '/CustomerInvoice/ShowDefItemList',
        datatype: "html",
        data: {
            CustomerId: $('#CustomerID').val(), DEFID:0
        },
        success: function (response) {
            debugger;
            $('#PackItemBody').html('');
            var data = response.data;
            for (var i = 0; i < response.data.length; i++) {
                var date = new Date(data[i].InvoiceDate);               
                var tempdate = (new Date(date).getMonth() + 1) + '/' + new Date(date).getDate() + '/' + new Date(date).getFullYear();
                var invoiceno = data[i].InvoiceNo;
                var html = '<tr id="tr_' + i + '"  >' +
                    '<td>' + invoiceno + '<input id="OhdnID_' + i + '" type="hidden"  value="' + data[i].ID + '" />  <input id="OhdnDEFRID_' + i + '""  value=' + data[i].DEFRID + ' type="hidden"/><input id="OhdnRemarks_' + i + '"" value="' + data[i].Remarks + '" type="hidden"><input id="hdnCustomerInvoiceID_' + i + '"  " value=' + data[i].CustomerInvoiceID + ' type="hidden"></td>' +
                    '<td>' + data[i].InvoiceDate + '</td>' +
                    '<td class="">' + data[i].Remarks + '<input id="OhdnAmount_' + i + '" value=' + data[i].Amount + ' type="hidden"><input id="OhdnBalance_' + i + '" value=' + data[i].Balance + ' type="hidden"></td>' +
                    '<td class="text-right">' + numberWithCommas(parseFloat(data[i].Amount).toFixed(_decimal)) + '</td>' +
                    '<td class="text-right">' + numberWithCommas(parseFloat(data[i].AmortizedAmount).toFixed(_decimal)) + '</td>' +
                    '<td class="text-right">' + numberWithCommas(parseFloat(data[i].Balance).toFixed(_decimal)) + '</td>';
                if (data[i].Allocated == true) {
                    html = html + '<td><input type="checkbox"   checked id="chkallocate_' + i + '" /></td>';
                }
                else {
                    html = html + '<td><input type="checkbox"  id="chkallocate_' + i + '" /></td>';
                }
                $('#PackItemBody').append(html);
            }

            $('#itempopup').modal('show');
        }
    });

}

function addDeferredItems() {

    var itemcount = $('#PackItemBody > tr').length;
    var idtext = 'chkallocate_';
      
    $('[id^=' + idtext + ']').each(function (index, item) {
        var checked = $('#chkallocate_' + index).prop('checked');
        if (checked ==true) {

            var Total = 0;
            var MainTotal = 0;

            var productTypeID = 0;
            var productName = ''
            var description = $('#OhdnRemarks_' + index).val();
            var exists = false;

            var UnitID = 1;
            var UnitName = 'PCS';
            var SalesRate = $('#OhdnBalance_' + index).val();
            var SalesRateFC = '';
            var DEFRID = $('#OhdnDEFRID_' + index).val();
            var Qty = 1;
            var Discount = 1;
            var DiscountAmount = 1;
            var DiscountAmountFC = 1;
            var SGST = 0;
            var SGSTAmt = 0;
            var CGST = 0;
            var CGSTAmt = 0;
            var IGST = 0;
            var IGSTAmt = 0;
            var Tax = 0;
            var TaxAmt = 0;


            var NetValue = $('#NetValue').val();
           
            var _readonly = "";
            var _localdnone = "";
            var _exportdnone = "";
            var _vatdnone = "";
            var _taxdetail = "";
            var _branchcurrencyreadonly = "";
            var _othercurrencyreadonly = "";
            getboxno();
            if ($('#BranchCurrency').val() == 'YES') {
                _othercurrencyreadonly = 'readonly';
            }
            else {
                _branchcurrencyreadonly = 'readonly';
            }

            if ($('#TaxType').val() == 1 && $('#InvoiceType').val() == 1) {
                _exportdnone = "d-none";
                _vatdnone = "d-none";
                _taxdetail = 'SGST % :' + SGST + ' CGST % :' + CGST;
            }
            else if ($('#TaxType').val() == 1 && $('#InvoiceType').val() == 2) {
                _localdnone = "d-none";
                _vatdnone = "d-none";
                _taxdetail = 'IGST % :' + IGST;
            }
            else if ($('#TaxType').val() == 1 && $('#InvoiceType').val() == 3) {
                _localdnone = "d-none";
                _vatdnone = "d-none";
                _taxdetail = '';
            }
            else {
                _localdnone = "d-none";
                _exportdnone = "d-none";
                _taxdetail = 'VAT % :' + Tax;
            }
            var RowCount = $('#ItemBody > tr').length; // parseInt($('#ItemRowCount').val());
            var taxhtml = '<input type="hidden" id="hdnSGST_' + RowCount + '" value="' + SGST + '" />' + '<input type="hidden" id="hdnSGSTAmount_' + RowCount + '" value="' + SGSTAmt + '" />';
            taxhtml = taxhtml + '<input type="hidden" id="hdnCGST_' + RowCount + '" value="' + CGST + '" />' + '<input type="hidden" id="hdnCGSTAmount_' + RowCount + '" value="' + CGSTAmt + '" />';
            taxhtml = taxhtml + '<input type="hidden" id="hdnIGST_' + RowCount + '" value="' + IGST + '" />' + '<input type="hidden" id="hdnIGSTAmount_' + RowCount + '" value="' + IGSTAmt + '" />';
            taxhtml = taxhtml + '<input type="hidden" id="hdnTaxPercent_' + RowCount + '" value="' + Tax + '" />' + '<input type="hidden" id="hdnTaxAmount_' + RowCount + '" value="' + TaxAmt + '" />';
            taxhtml = taxhtml + '<input type="hidden" id="NetValue_' + RowCount + '" value="' + NetValue + '" /><input type="hidden" id="DEFRID_' + RowCount + '" value="0" /><input type="hidden" id="ForDEFRID_' + RowCount + '" value="' + DEFRID + '" />';

            var RowHtml = '<tr id="tr_' + RowCount + '"><td> <span id="BoxNo_' + RowCount + '">' + $('#BoxNo').html() + '</span><input type="hidden"   class="hdndeleted" value="false" id="hdnItemdeleted_' + RowCount + '" /><input type="hidden" id="ProductID_' + RowCount + '"     value="' + productTypeID + '"/> ' + taxhtml + ' </td>';
            RowHtml = RowHtml + '<td><input type="text" class="form-select" salesrate="' + SalesRate + '"  id="ProductName_' + RowCount + '" value="' + productName + '" /><span class="clstax color_red text-right"  id="spantaxdetail_' + RowCount + '">' + _taxdetail + '</span></td>';
            RowHtml = RowHtml + '<td><input type = "text" class="form-control" id = "Description_' + RowCount + '"  value = "' + description + '" /></td>';
            RowHtml = RowHtml + '<td><input type = "hidden" class="form-control  textright clsbox" ' + _readonly + ' id = "hdnUnitID_' + RowCount + '"  value = "' + UnitID + '" /><input type="text" class="form-control clsbox"   id="UnitName_' + RowCount + '" value="' + UnitName + '" /></td>';
            RowHtml = RowHtml + '<td><input type = "text" class="form-control  textright clsbox" ' + _readonly + ' id = "Qty_' + RowCount + '"  value = "' + Qty + '" onchange="calculateNetvalue1(' + RowCount + ')" /></td>';

            RowHtml = RowHtml + '<td><input type = "text" class="form-control  textright clsrate" ' + _branchcurrencyreadonly + ' id = "SalesRate_' + RowCount + '"  value = "' + SalesRate + '" onchange="calculateNetvalue1(' + RowCount + ')"  /></td>';
            RowHtml = RowHtml + '<td><input type = "text" class="form-control  textright clsrate" ' + _othercurrencyreadonly + ' id = "SalesRateFC_' + RowCount + '"  value = "' + SalesRateFC + '" onchange="calculateLocalRate1(' + RowCount + ')"  /></td>';
            RowHtml = RowHtml + '<td><input type = "text" class="form-control  textright clsbox" ' + _readonly + ' id = "Discount_' + RowCount + '"  value = "' + Discount + '" onchange="calculateNetvalue1(' + RowCount + ')" /></td>';
            RowHtml = RowHtml + '<td><input type = "text" class="form-control  textright clsbox" ' + _readonly + ' id = "DiscountAmount_' + RowCount + '"  value = "' + DiscountAmount + '" onchange="calculateNetvalue1(' + RowCount + ')" />' +
                '<input type="hidden" class="form-control  textright clsbox" ' + _readonly + ' id = "hdnDiscountAmount_' + RowCount + '"  value = "' + DiscountAmount + '"   />' +
                '<input type="hidden" class="form-control  textright clsbox" ' + _readonly + ' id = "hdnDiscountAmountFC_' + RowCount + '"  value = "' + DiscountAmountFC + '"  /></td> ';

            RowHtml = RowHtml + '<td class="taxlocal text-right ' + _localdnone + '" ><span id = "spanSGSTAmount_' + RowCount + '">' + numberWithCommas(SGSTAmt) + '</span></td>';
            RowHtml = RowHtml + '<td class="taxlocal text-right ' + _localdnone + '" ><span id = "spanCGSTAmount_' + RowCount + '">' + numberWithCommas(CGSTAmt) + '</span></td>';
            RowHtml = RowHtml + '<td class="taxexport text-right ' + _exportdnone + '" ><span id = "spanIGSTAmount_' + RowCount + '">' + numberWithCommas(IGSTAmt) + '</span></td>';
            RowHtml = RowHtml + '<td class="taxvat text-right ' + _vatdnone + '" ><span id = "spanTaxAmount_' + RowCount + '">' + numberWithCommas(TaxAmt) + '</span></td>';
            RowHtml = RowHtml + '<td class="text-right"><span id = "spanNetValue_' + RowCount + '">' + numberWithCommas(NetValue) + '</span></td>';
            RowHtml = RowHtml + '<td><div class="d-flex gap-3"> <a href="javascript:void(0)" class="text-danger" onclick="deleteItemtrans(this)"  class="deleteallocrow" id="del_' + RowCount + '"><i class="mdi mdi-delete font-size-18"></i></a></div></td>';
            RowHtml = RowHtml + '</tr>';
            $('#ItemBody').append(RowHtml);
            calculatenettotal();
            $("#ProductName_" + RowCount).autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: '/Product/GetProductName',
                        datatype: "json",
                        data: {
                            term: request.term, ProductTypeID: 0
                        },
                        success: function (data) {
                            response($.map(data, function (val, item) {
                                return {
                                    label: val.ProductName,
                                    value: val.ProductID,
                                    UnitId: val.UnitId,
                                    SalesRate: val.SalesRate,
                                    UnitName: val.UnitName,
                                    SGST: val.SGST,
                                    CGST: val.CGST,
                                    IGST: val.IGST,
                                    TaxPercent: val.TaxPercent


                                }
                            }))
                        }
                    })
                }, minLength: 0,
                autoFocus: false,
                focus: function (e, i) {
                    e.preventDefault();
                    $("#ProductName_" + RowCount).val(i.item.label);
                    $('#ProductID_' + RowCount).val(i.item.value);
                    $('#hdnUnitID_' + RowCount).val(i.item.UnitId);
                    $('#UnitName_' + RowCount).val(i.item.UnitName);
                    var salesrate = $(this).attr('salesrate');
                    $('#SalesRate_' + RowCount).val(numberWithCommas(salesrate));
                    $('#hdnSGST_' + RowCount).val(i.item.SGST);
                    $('#hdnCGST_' + RowCount).val(i.item.CGST);
                    $('#hdnIGST_' + RowCount).val(i.item.IGST);
                    $('#hdnTaxPercent_' + RowCount).val(i.item.TaxPercent);
                    showtax1(i.item, RowCount);
                    calculateNetvalue1(RowCount);
                },
                select: function (e, i) {
                    e.preventDefault();
                    $("#ProductName_" + RowCount).val(i.item.label);
                    $('#ProductID_' + RowCount).val(i.item.value);
                    $('#hdnUnitID_' + RowCount).val(i.item.UnitId);
                    $('#UnitName_' + RowCount).val(i.item.UnitName);
                    var salesrate = $(this).attr('salesrate');
                    $('#SalesRate_' + RowCount).val(numberWithCommas(salesrate));
                    $('#hdnSGST_' + RowCount).val(i.item.SGST);
                    $('#hdnCGST_' + RowCount).val(i.item.CGST);
                    $('#hdnIGST_' + RowCount).val(i.item.IGST);
                    $('#hdnTaxPercent_' + RowCount).val(i.item.TaxPercent);
                    showtax1(i.item, RowCount);
                    calculateNetvalue1(RowCount);

                }
            });

        }
    })

}
(function ($) {

    'use strict';
    function initformControl() {
        $('#DueDate').datepicker({
            dateFormat: 'dd-mm-yy',
        }).on('changeDate', function (e) {
            $(this).datepicker('hide');
        });

        $('#InvoiceDate').change(function () {
            $('#DueDate').val($('#InvoiceDate').val());
        })
        $("#AcHead").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/Accounts/GetHeadsForCash',
                    datatype: "json",
                    data: {
                        term: request.term
                    },
                    success: function (data) {
                        response($.map(data, function (val, item) {
                            return {
                                label: val.AcHead,
                                value: val.AcHead,
                                AcHeadID: val.AcHeadID
                            }
                        }))
                    }
                })
            }, minLength: 1,
            autoFocus: false,
            focus: function (e, i) {
                e.preventDefault();
                $("#AcHead").val(i.item.label);
                $('#AcHeadId').val(i.item.AcHeadID);
                $('#AcHead').attr('label', i.item.label);
            },
            select: function (e, i) {
                e.preventDefault();
                $("#AcHead").val(i.item.label);
                $('#AcHeadId').val(i.item.AcHeadID);
                $('#AcHead').attr('label', i.item.label);
            }
        });

        $('#btnsearch').click(function () {

            if ($('#CustomerID').val() == 0 || $('#CustomerID').val() == '' || $('#CustomerID').val() == null) {
                $('#CustomerName').focus();
                return false;
            }
            else if ($('#SelectedValues').val() == null) {
                $('#SelectedValue').focus();
                
                Swal.fire("Data Validation", "Select Courier type!", "warning");
                return false;
            }

            $.ajax({
                type: 'POST',
                url: '/CustomerInvoice/ShowItemList',
                datatype: "html",
                data: {
                    CustomerId: $('#CustomerID').val(), FromDate: $('#FromDate').val(), ToDate: $('#ToDate').val(), MovementId: $('#SelectedValues').val(), InvoiceId:$('#CustomerInvoiceID').val()
                },
                success: function (data) {
                    debugger;
                    $("#listContainer").html(data);
                    $("#CustomerName").attr('CustomerId', $('#CustomerID').val());
                    $('#btnsave1').removeAttr('disabled');
                    var max = $('#details > tbody > tr').length - 7;

                    if (max == 0) {
                        
                        Swal.fire("Data Validation", "AWB are not found!", "warning");
                    }
                    else {
                        ValidateTotal();
                    }

                }
            });

        });

        if ($('#CustomerInvoiceID').val() > 0) {
            $('#btnsave').val('Update');
            $('#CustomerName').attr('disabled', 'disabled');
            $('#InvoiceType').val($('#InvoiceType').attr('svalue')).trigger('change');
            $('#InvoiceType').trigger('change');
            calculatenettotal();
            settaxtype();
            setCurrencywiseSalesInput();
            $('#PaymentModeId').attr('disabled', 'disabled');
            $('#InvoiceType').attr('disabled', 'disabled');
            $('#CurrencyID').attr('disabled', 'disabled');
            setTimeout(function () {
                bindproductgrid();
            },200)
                            
            $('#divothermenu').removeClass('hide');
            $('#InvoiceType').focus();
        }
        else {
       
            $('#InvoiceType').val($('#InvoiceType').attr('svalue')).trigger('change');
            $('#InvoiceType').trigger('change');
            settaxtype();
            setCurrencywiseSalesInput();
            $('#InvoiceType').focus();
        }
       

    }
    function init() {
        initformControl();
    }
    window.addEventListener(
        "load",
        function () {
            var t = document.getElementsByClassName("needs-validation");
            Array.prototype.filter.call(t, function (e) {
                e.addEventListener(
                    "submit",
                    function (t) {

                        if (false === e.checkValidity()) {
                            e.classList.add("was-validated");
                        }
                        else {
                            t.preventDefault();
                            t.stopPropagation();
                            e.classList.remove("was-validated");
                              SaveCustomerInvoice();

                        }
                    },
                    !1
                );
            });
        },
        !1
    );
    $(document).ready(function () {
        init();
        $("#CustomerName").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/CustomerMaster/GetCustomerName',
                    datatype: "json",
                    data: {
                        term: request.term
                    },
                    success: function (data) {
                        response($.map(data, function (val, item) {
                            return {
                                label: val.CustomerName,
                                value: val.CustomerName,
                                CustomerId: val.CustomerID,
                                type: val.CustomerType,
                                CustomerPhoneNo: val.CustomerPhoneNo,
                                Email: val.Email,
                                Address1: val.Address1,
                                Address2: val.Address2,
                                Address3: val.Address3,
                                CityName: val.CityName,
                                CountryName: val.CountryName
                            }
                        }))
                    }
                })
            },
            minLength: 1,
            autoFocus: false,
            focus: function (e, ui) {
                e.preventDefault();
                $("#CustomerName").val(ui.item.label);

                $('#CustomerID').val(ui.item.CustomerId);
                $('#CustomerPhoneNo').val(ui.item.CustomerPhoneNo);
                $('#CustomerEmailID').val(ui.item.Email);
                var address = getaddress(ui.item);
                $('#CustomerAddress').val(address);
            },
            select: function (e, ui) {
                e.preventDefault();
                $("#CustomerName").val(ui.item.label);

                $('#CustomerID').val(ui.item.CustomerId);
                $('#CustomerPhoneNo').val(ui.item.CustomerPhoneNo);
                $('#CustomerEmailID').val(ui.item.Email);
                var address = getaddress(ui.item);
                $('#CustomerAddress').val(address);
            },

        });
        $('#btnShowDFItem').click(function () {
            if ($('#PaymentModeId').val() == '5') {
                getcustomerdetail();
            }
        })
        $('#CustomerName').change(function () {
            if ($('#PaymentModeId').val() == '5') {
                getcustomerdetail();
            }
           
        })

        $('#CustomerPhoneNo').blur(function () {
            if ($('#PaymentModeId').val() == '3') {
                getcustomerdetail();
            }

        })
        $('#InvoiceType').change(function () {
            settaxtype();
          
        })
        $('#ClearItem').click(function () {

            $('#ProductName').val('');

            $('#UnitID').val(1);
            $('#UnitName').val('');
            $('#SalesRate').val(0);
            $('#SalesRateFC').val(0);
            $('#Qty').val(1);
            $('#ProductID').val(0);
            $('#Discount1').val(0);
            $('#NetValue').val(0);
            $('#nSGST').val(0);
            $('#nSGSTAmount').val(0);
            $('#CGST').val(0);
            $('#nCGSTAmount').val(0);
            $('#nIGST').val(0);
            $('#nIGSTAmount').val(0);
            $('#nTaxPercent').val(0);
            $('#nTaxAmount').val(0);
            $('#SGSTAmount1').html('');
            $('#CGSTAmount1').html('');
            $('#IGSTAmount1').html('');
            $('#Description').html('');
            $('#TaxAmount1').html('');
            $('#spantaxdetail').html('');
            $('#NetValue1').html('');
            $('#ProductName').focus();
        });
        $('#AddItem').click(function () {
            debugger;
            var Total = 0;
            var MainTotal = 0;
            var productTypeID = $("#ProductTypeID1").val();
            var productid= $("#ProductID").val();
            var productName = $("#ProductName").val();
            var description = $('#Description').val();
            var exists = false;
            
            var UnitID = $("#UnitID").val();
            var UnitName = $('#UnitName').val();
            var SalesRate = $('#SalesRate').val();
            var SalesRateFC = $('#SalesRateFC').val();
            var Qty = $('#Qty').val();
            var Discount = $('#DiscountPercent1').val();
            var DiscountAmount = $('#hdnDiscountAmount1').val();
            var DiscountAmountFC = $('#hdnDiscountAmount1FC').val();
            var SGST = $('#nSGST').val();
            var SGSTAmt = $('#nSGSTAmount').val();
            var CGST = $('#nCGST').val();
            var CGSTAmt = $('#nCGSTAmount').val();
            var IGST = $('#nIGST').val();
            var IGSTAmt = $('#nIGSTAmount').val();
            var Tax = $('#nTaxPercent').val();
            var TaxAmt = $('#nTaxAmount').val();

           
            var NetValue = $('#NetValue').val();
            if (productName == "") {
                Swal.fire('Data Validation', 'Please Enter Product Name!');
                return;
            }
            var _readonly = "";
            var _localdnone = "";
            var _exportdnone = "";
            var _vatdnone = "";
            var _taxdetail = "";
            var _branchcurrencyreadonly = "";
            var _othercurrencyreadonly = "";

            if ($('#BranchCurrency').val() == 'YES') {
                _othercurrencyreadonly = 'readonly';
            }
            else {
                _branchcurrencyreadonly = 'readonly';
            }

            if ($('#TaxType').val() == 1 && $('#InvoiceType').val() == 1) {
                _exportdnone = "d-none";
                _vatdnone = "d-none";
                _taxdetail = 'SGST % :' + SGST + ' CGST % :' + CGST;
            }
            else if ($('#TaxType').val() == 1 && $('#InvoiceType').val() == 2 ) {
                _localdnone = "d-none";
                _vatdnone = "d-none";
                _taxdetail = 'IGST % :' + IGST;
            }
            else if ($('#TaxType').val() == 1 && $('#InvoiceType').val() == 3) {
                _localdnone = "d-none";
                _vatdnone = "d-none";
                _taxdetail = '';
            }
            else {
                _localdnone = "d-none";
                _exportdnone = "d-none";
                _taxdetail = 'VAT % :' + Tax;
            }

            
            var RowCount = $('#ItemBody > tr').length; // parseInt($('#ItemRowCount').val());
            var producttypehtml = '<td><select class="form-select-sm"  id="ProductTypeID1_' + RowCount + '" width="100px" style="border-radius: 5px;">' + $('#ProductTypeID1').html() + '</select></td>';
            var taxhtml = '<input type="hidden" id="hdnSGST_' + RowCount + '" value="' + SGST + '" />' + '<input type="hidden" id="hdnSGSTAmount_' + RowCount + '" value="' + SGSTAmt + '" />';
            taxhtml = taxhtml + '<input type="hidden" id="hdnCGST_' + RowCount + '" value="' + CGST + '" />' + '<input type="hidden" id="hdnCGSTAmount_' + RowCount + '" value="' + CGSTAmt + '" />';
            taxhtml = taxhtml + '<input type="hidden" id="hdnIGST_' + RowCount + '" value="' + IGST + '" />' + '<input type="hidden" id="hdnIGSTAmount_' + RowCount + '" value="' + IGSTAmt + '" />';
            taxhtml = taxhtml + '<input type="hidden" id="hdnTaxPercent_' + RowCount + '" value="' + Tax + '" />' + '<input type="hidden" id="hdnTaxAmount_' + RowCount + '" value="' + TaxAmt + '" />';
            taxhtml = taxhtml + '<input type="hidden" id="NetValue_' + RowCount + '" value="' + NetValue + '" />';

            var RowHtml = '<tr id="tr_' + RowCount + '"><td> <span id="BoxNo_' + RowCount + '">' + $('#BoxNo').html() + '</span><input type="hidden"   class="hdndeleted" value="false" id="hdnItemdeleted_' + RowCount + '" /><input type="hidden" id="ProductID_' + RowCount + '"     value="' + productid + '"/> ' + taxhtml + ' </td>';
            RowHtml = RowHtml + producttypehtml;
            RowHtml = RowHtml + '<td><input type="text" class="form-select"   id="ProductName_' + RowCount + '" value="' + productName + '" /><span class="clstax color_red text-right"  id="spantaxdetail_' + RowCount + '">' + _taxdetail + '</span></td>';
            RowHtml = RowHtml + '<td><input type = "text" class="form-control" id = "Description_' + RowCount + '"  value = "' + description + '" /></td>';
            RowHtml = RowHtml + '<td><input type = "hidden" class="form-control  textright clsbox" ' + _readonly + ' id = "hdnUnitID_' + RowCount + '"  value = "' + UnitID + '" /><input type="text" class="form-control clsbox"   id="UnitName_' + RowCount + '" value="' + UnitName + '" /></td>';
            RowHtml = RowHtml + '<td><input type = "text" class="form-control  textright clsbox" ' + _readonly + ' id = "Qty_' + RowCount + '"  value = "' + Qty + '" onchange="calculateNetvalue1(' + RowCount + ')" /></td>';
      
            RowHtml = RowHtml + '<td><input type = "text" class="form-control  textright clsrate" ' + _branchcurrencyreadonly + ' id = "SalesRate_' + RowCount + '"  value = "' + SalesRate + '" onchange="calculateNetvalue1(' + RowCount + ')"  /></td>';
            RowHtml = RowHtml + '<td><input type = "text" class="form-control  textright clsrate" ' + _othercurrencyreadonly + ' id = "SalesRateFC_' + RowCount + '"  value = "' + SalesRateFC + '" onchange="calculateLocalRate1(' + RowCount + ')"  /></td>';
            RowHtml = RowHtml + '<td><input type = "text" class="form-control  textright clsbox" ' + _readonly + ' id = "Discount_' + RowCount + '"  value = "' + Discount + '" onchange="calculateNetvalue1(' + RowCount + ')" /></td>';
            RowHtml = RowHtml + '<td><input type = "text" class="form-control  textright clsbox" ' + _readonly + ' id = "DiscountAmount_' + RowCount + '"  value = "' + DiscountAmount + '" onchange="calculateNetvalue1(' + RowCount + ')" />' + 
                '<input type="hidden" class="form-control  textright clsbox" ' + _readonly + ' id = "hdnDiscountAmount_' + RowCount + '"  value = "' + DiscountAmount + '"   />' +
                '<input type="hidden" class="form-control  textright clsbox" ' + _readonly + ' id = "hdnDiscountAmountFC_' + RowCount + '"  value = "' + DiscountAmountFC + '"  /></td> ';
           
            RowHtml = RowHtml + '<td class="taxlocal text-right ' + _localdnone + '" ><span id = "spanSGSTAmount_' + RowCount + '">' +  numberWithCommas(SGSTAmt) + '</span></td>';
            RowHtml = RowHtml + '<td class="taxlocal text-right ' + _localdnone + '" ><span id = "spanCGSTAmount_' + RowCount + '">' + numberWithCommas(CGSTAmt) + '</span></td>';
            RowHtml = RowHtml + '<td class="taxexport text-right ' + _exportdnone + '" ><span id = "spanIGSTAmount_' + RowCount + '">' + numberWithCommas(IGSTAmt) + '</span></td>';
            RowHtml = RowHtml + '<td class="taxvat text-right ' + _vatdnone + '" ><span id = "spanTaxAmount_' + RowCount + '">' + numberWithCommas(TaxAmt) + '</span></td>';
            RowHtml = RowHtml + '<td class="text-right"><span id = "spanNetValue_' + RowCount + '">' + numberWithCommas(NetValue) + '</span></td>';
            RowHtml = RowHtml + '<td><div class="d-flex gap-3"> <a href="javascript:void(0)" class="text-danger" onclick="deleteItemtrans(this)"  class="deleteallocrow" id="del_' + RowCount + '"><i class="mdi mdi-delete font-size-18"></i></a></div></td>';
            RowHtml = RowHtml + '</tr>';
            $('#ItemBody').append(RowHtml);
            $('#ProductTypeID1_' + RowCount).val($('#ProductTypeID1').val()).trigger('change');

            calculatenettotal();
            $("#ProductName_" + RowCount).autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: '/Product/GetProductName',
                        datatype: "json",
                        data: {
                            term: request.term ,ProductTypeID :$('#ProductTypeID1_' + RowCount).val()
                        },
                        success: function (data) {
                            response($.map(data, function (val, item) {
                                return {
                                    label: val.ProductName,
                                    value: val.ProductID,
                                    UnitId: val.UnitId,
                                    SalesRate: val.SalesRate,
                                    UnitName: val.UnitName,
                                    SGST: val.SGST,
                                    CGST: val.CGST,
                                    IGST: val.IGST,
                                    TaxPercent :val.TaxPercent


                                }
                            }))
                        }
                    })
                }, minLength: 0,
                autoFocus:false,
                focus: function (e, i) {
                    e.preventDefault();
                    $("#ProductName_" + RowCount).val(i.item.label);
                    $('#ProductID_' + RowCount).val(i.item.value);
                    $('#hdnUnitID_' + RowCount).val(i.item.UnitId);
                    $('#UnitName_' + RowCount).val(i.item.UnitName);
                    $('#SalesRate_' + RowCount).val(numberWithCommas(i.item.SalesRate));
                    $('#hdnSGST_' + RowCount).val(i.item.SGST);
                    $('#hdnCGST_' + RowCount).val(i.item.CGST);
                    $('#hdnIGST_' + RowCount).val(i.item.IGST);
                    $('#hdnTaxPercent_' + RowCount).val(i.item.TaxPercent);
                    showtax1(i.item, RowCount);
                    calculateNetvalue1(RowCount);
                },
                select: function (e, i) {
                    e.preventDefault();
                    $("#ProductName_" + RowCount).val(i.item.label);
                    $('#ProductID_' + RowCount).val(i.item.value);
                    $('#hdnUnitID_' + RowCount).val(i.item.UnitId);
                    $('#UnitName_' + RowCount).val(i.item.UnitName);
                    $('#SalesRate_' + RowCount).val(numberWithCommas(i.item.SalesRate));
                    $('#hdnSGST_' + RowCount).val(i.item.SGST);
                    $('#hdnCGST_' + RowCount).val(i.item.CGST);
                    $('#hdnIGST_' + RowCount).val(i.item.IGST);
                    $('#hdnTaxPercent_' + RowCount).val(i.item.TaxPercent);
                    showtax1(i.item, RowCount);
                    calculateNetvalue1(RowCount);

                }
            });


            $('#ItemRowCount').val(RowCount + 1);
            $('#ProductName').val('');
            $('#Description').val('');
            $('#UnitID').val(1);
            $('#UnitName').val('');
            $('#SalesRate').val(0);
            $('#SalesRateFC').val(0);
                $('#Qty').val(1);
            $('#ProductID').val(0);
            $('#DiscountAmount1').val(0);
            $('#DiscountAmount1FC').val(0);
            $('#DiscountPercent1').val(0);
            $('#NetValue').val(0);
            $('#nSGST').val(0);
            $('#nSGSTAmount').val(0);
            $('#nCGST').val(0);
            $('#nCGSTAmount').val(0);
            $('#nIGST').val(0);
            $('#nIGSTAmount').val(0);
            $('#nTaxPercent').val(0);
            $('#nTaxAmount').val(0);
            $('#SGSTAmount1').html('');
            $('#CGSTAmount1').html('');
            $('#IGSTAmount1').html('');
            $('#Description').html('');
            $('#TaxAmount1').html('');
            $('#spantaxdetail').html('');
            $('#NetValue1').html('');
            $('#ProductName').focus();

         
  


        });
        $("#ProductName").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/Product/GetProductName',
                    datatype: "json",
                    data: {
                        term: request.term, ProductTypeID: $('#ProductTypeID1').val()
                    },
                    success: function (data) {
                        response($.map(data, function (val, item) {
                            return {
                                label: val.ProductName,
                                value: val.ProductID,
                                UnitId: val.UnitId,
                                SalesRate: val.SalesRate,
                                UnitName: val.UnitName,
                                SGST: val.SGST,
                                CGST: val.CGST,
                                IGST: val.IGST,
                                TaxPercent: val.TaxPercent

                            }
                        }))
                    }
                })
            },
            minLength: 1,
            autoFocus: false,
            focus: function (event, ui) {
                $("#ProductName").val(ui.item.label);
                $("#ProductID").val(ui.item.value);
             
                $('#UnitID').val(ui.item.UnitId);
                $('#UnitName').val(ui.item.UnitName);
                $('#SalesRate').val(numberWithCommas(ui.item.SalesRate));
                $('#ProductID').val(ui.item.value);
                $('#nSGST').val(ui.item.SGST);
                $('#nCGST').val(ui.item.CGST);
                $('#nIGST').val(ui.item.IGST);
                $('#nTaxPercent').val(ui.item.TaxPercent);
                if ($('#Qty').val() == '')
                    $('#Qty').val(1);
                calculateNetvalue();
                showtax(ui.item);
            },
            select: function (e, ui) {
                e.preventDefault();
                $("#ProductName").val(ui.item.label);
            
                $('#UnitID').val(ui.item.UnitId);
                $('#UnitName').val(ui.item.UnitName);
                $('#SalesRate').val(numberWithCommas(ui.item.SalesRate));
                $('#ProductID').val(ui.item.value);
                $('#nSGST').val(ui.item.SGST);
                $('#nCGST').val(ui.item.CGST);
                $('#nIGST').val(ui.item.IGST);
                $('#nTaxPercent').val(ui.item.TaxPercent);
                if ($('#Qty').val() == '')
                    $('#Qty').val(1);
                calculateNetvalue();
                showtax(ui.item);
            },

        });


        $('#PaymentModeId').change(function () {

            if ($('#PaymentModeId').val() == 3) {//Account
                $('#AcHeadId').val($('#CustomerControlAcheadId').val());
                $('#AcHead').val($('#CustomerControlAchead').val());
                $('#AcHead').attr('disabled', 'disabled');
                $('#lblchartaccount').html('Chart of Account');
                $('#trnewrow').removeClass('hide');
            }
            else if ($('#PaymentModeId').val() == 4) {//FOc Account
                $('#AcHeadId').val($('#FOCControlAcheadId').val());
                $('#AcHead').val($('#FOCControlAchead').val());
                $('#AcHead').attr('disabled', 'disabled');
                $('#lblchartaccount').html('Chart of Account');
                $('#trnewrow').removeClass('hide');
            }
            else if ($('#PaymentModeId').val() == 5) {//Deffered REvenue 
                $('#AcHeadId').val($('#DFRControlAcheadId').val());
                $('#AcHead').val($('#DFRControlAchead').val());
                $('#AcHead').attr('disabled', 'disabled');
                $('#lblchartaccount').html('Chart of Account');
                $('#trnewrow').addClass('hide');
            }
            else {
                $('#AcHeadId').val(0);
                $('#AcHead').val('');
                $('#AcHead').removeAttr('disabled');
                $('#lblchartaccount').html('Cash Account');
                $('#trnewrow').removeClass('hide');
            }

        });

        $('#txtDescription').focus(function () {
            if ($('#PaymentModeId').val() == 5) {
                showitempopup();
            }
        });

        //$('#CustomerName').change(function () {
        //    showitempopup();
        //})
    })

})(jQuery)