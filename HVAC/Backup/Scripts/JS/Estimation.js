 

var _decimal = $('#hdncompanydecimal').val();

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
function numberWithCommas(x) {
    if (x != null)
        return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    else {
        return '';
    }
}

function calculationchargeable() {
    debugger;
    var paymentdays = $('#PaymentDays').val();
    var freedays = $('#FreeServiceDays').val();

    if (paymentdays == '')
        paymentdays = 0;
    if (freedays == '')
        freedays = 0;
    var chargablemonth = Math.round((parseInt(paymentdays) - parseInt(freedays))/30);
    $('#ChargeableMonth').val(chargablemonth);
    var finchargepercent = $('#FinChargePercent').val();
    if (finchargepercent == '') {
        finchargepercent = '.017500';
        $('#FinChargePercent').val(finchargepercent);
    }
    var equipmenttotal =parsenumeric($('#EquipmentsTotal'));
    if (equipmenttotal == null)
        equipmenttotal = 0;

    var accessoriestotal = parsenumeric($('#AccessoriesTotal'));
    if (accessoriestotal == null)
        accessoriestotal = 0;


    var _FreightTotal = parsenumeric($('#FreightTotal'));
    var grouptotal1 = parseFloat(equipmenttotal) + parseFloat(accessoriestotal) + parseFloat(_FreightTotal);
    var financecharge = parseFloat(grouptotal1) * (parseFloat(finchargepercent));
    var financepermonth = parseFloat(financecharge) /parseInt(chargablemonth);
    var finPerMonthPercent = (parseFloat(finchargepercent) / 100.0) / parseInt(chargablemonth);
    $('#FinCharge').val(Math.round(financecharge.toFixed(2)));
    $('#FinPerMonthPercent').val(parseFloat(finPerMonthPercent).toFixed(6));
    //var FinPerMonth = parseFloat(parseFloat(grouptotal1) * (parseFloat(finchargepercent) / 100.0)).toFixed(2);
    var FinPerMonth = parseFloat(grouptotal1) * parseFloat($('#FinPerMonthPercent').val());
    $('#FinPerMonth').val(Math.round(parseFloat(financepermonth).toFixed(2)));

}
function ApplyFinanceCharge() {
    var idtext = 'quodtr_'
    var finchargefound = false;
    var _exrate = '0.39';
    var itemcount = $('#EstimationDetailTable > tbody > tr').length;
    $('[id^=' + idtext + ']').each(function (index, item) {
        if ($('#QDeleted_' + index).val() != 'true') {
            var _estimationcategoryid = $('#QEstimationCategoryID_' + index).val();
            if (_estimationcategoryid == 4) {
                finchargefound = true;
                $('#QtxtRate_' + index).val($('#FinPerMonth').val());
                $('#QtxtExRate_' + index).val($('#FinPerMonth').val());
                $('#QtxtQty_' + index).val($('#ChargeableMonth').val());
                $('#QCurrencyID_' + index).val(3);
                $('#QtxtExRate_' + index).val(_exrate);
                var _lvalue = parseFloat(parsenumeric($('#QtxtRate_' + index))) * parseFloat(parsenumeric($('#QtxtQty_' + index))) * parseFloat(_exrate);
                $('#QtxtFValue_' + index).val(parsenumeric($('#FinCharge')));
                $('#QtxtLValue_' + index).val(_lvalue); //omr value
                $('#FinanceChargesTotal').val(parsenumeric($('#FinCharge')));
                calculatelandingcost();
            }
            
        }
        if (itemcount == (index + 1))
        {
            if (finchargefound == false) {              
                var _Equipmentname = 'Finance Charges';
                var _EquipmentId = 0;
                var _Model = '';
                var equipmententry = {
                    EstimationDetailID: 0,
                    EstimationID: $('#EstimationID').val(),
                    CategoryName: 'Finance Charges',
                    EstimationCategoryID: 4,
                    EquipmentID: _EquipmentId,
                    Description: _Equipmentname,
                    Model: _Model,
                    UnitID: 1,
                    CurrencyID: 3,
                    CurrencyCode: 'USD',
                    UnitName: 'Nos',
                    ExchangeRate:_exrate,
                    Qty: $('#ChargeableMonth').val(),
                    Rate: $('#FinPerMonth').val(),
                    FValue: $('#FinCharge').val(),
                    LValue: parseFloat(parsenumeric($('#FinCharge'))) * parseFloat(0.39),
                    Deleted:false
                }
                $('#FinanceChargesTotal').val(parsenumeric($('#FinCharge')));
                var quotationdetails = [];
                for (i = 0; i < itemcount; i++) {
                    var quotationdetail = {
                        EstimationDetailID: $('#QEstimationDetailID_' + i).val(),
                        EstimationID: $('#EstimationID').val(),
                        EstimationCategoryID: $('#QEstimationCategoryID_' + i).val(),
                        EstimationMasterID: $('#QEstimationMasterID_' + i).val(),
                        CategoryName: $('#QtxtCategoryName_' + i).val(),
                        UnitName: $('#QUnitName_' + i).val(),
                        UnitID: $('#QUnitID_' + i).val(),
                        CurrencyID: $('#QCurrencyID_' + i).val(),
                        CurrencyCode: $('#QCurrencyName_' + i).val(),
                        ExchangeRate: $('#QtxtExRate_' + i).val(),
                        EquipmentID: $('#QEquipmentID_' + i).val(),
                        Description: $('#QtxtDescription_' + i).val(),
                        Model: $('#QtxtModel_' + i).val(),
                        Qty: $('#QtxtQty_' + i).val(),
                        Rate: parsenumeric($('#QtxtRate_' + i)),
                        FValue: parsenumeric($('#QtxtFValue_' + i)),
                        LValue: parsenumeric($('#QtxtLValue_' + i)),
                        Deleted: $('#QDeleted_' + i).val(),
                        Roworder: $('#QRoworder_' + i).val(),
                        RowType: $('#QRowType_' + i).val()

                    }

                    quotationdetails.push(quotationdetail);

                }


                $.ajax({
                    type: "POST",
                    url: '/Estimation/AddItem/',
                    datatype: "html",
                    data: { invoice: equipmententry, index: -1, Details: JSON.stringify(quotationdetails), WorkingDays: $('#txtpaymentdays1').val() },
                    success: function (data) {
                        $("#EstimationDetailContainer").html(data);
                        $(obj).removeAttr('disabled');
                        $('#QEquipmentCategory').focus()
                        calculatequotationvalue();
                        clearDetail();
                        calculatelandingcost();
                    }
                });

            }

        }
        
    });
}

function ApplyMargin() {
    var _marginpercent = $('#MarginPercent').val();
    if (_marginpercent == '') {
        _marginpercent = 0;
    }

    if (parseFloat(_marginpercent) > 0) {
        $('#spanmarginpercent').html('Margin ' + $('#MarginPercent').val() + '%');
        calculatequotationvaluenew();
        $('#trmargin').removeClass('d-xl-none');
        $('#trselling').removeClass('d-xl-none');
    }
    else {
        $('#spanmarginpercent').html('');
        //calculatequotationvaluenew();
        $('#trmargin').addClass('d-xl-none');
        $('#trselling').addClass('d-xl-none');
    }
}
function calculatelandingcost() {
    debugger;
    var exrate = "0.39";
    var equipmenttotal = parsenumeric($('#EquipmentsTotal'));
    if (equipmenttotal == null)
        equipmenttotal = 0;

    var accessoriestotal = parsenumeric($('#AccessoriesTotal'));
    if (accessoriestotal == null)
        accessoriestotal = 0;

    var _FreightTotal = parsenumeric($('#FreightTotal'));
    var _financetotal = parsenumeric($('#FinanceChargesTotal'));
    var landingcost = parseFloat(exrate) * (parseFloat(accessoriestotal) + parseFloat(equipmenttotal) + parseFloat(_FreightTotal) + parseFloat(_financetotal));
    $('#LandingCost').val(parseFloat(landingcost).toFixed());
    if (parseFloat(landingcost) > 0)
    {
        $('#spanlandingcost').html('Landing Cost:' + numberWithCommas(parseFloat(landingcost).toFixed(2)));
    }
    else {
        $('#spanlandingcost').html('');
    }

}
 //add new item calculation
function calculatevalue1() {
    debugger;
    var rate = parsenumeric($('#QtxtRate'));//.val();
    var qty = parsenumeric($('#QtxtQty'));// 
    if (rate == '')
        rate = 0;
    if (qty == '')
        qty = 0;
    $('#QtxtRate').val(parseFloat(rate).toFixed(_decimal));

    var currencyid = $('#QCurrencyID').val();
    if (currencyid == $('#DefaultCurrencyID').val()) //OMR later default currency
    {
        $('#QtxtFValue').val(0);
    }
    else {
        var value = parseFloat(rate) * parseFloat(qty);
        value = parseFloat(value).toFixed(2);
        $('#QtxtFValue').val(value);
    }
        
    
    if ($('#DefaultCurrencyID').val() == $('#QCurrencyID').val()) {
        $('#QtxtExRate').val(1);
    }
    var exrate = $('#QtxtExRate').val();
    value = parseFloat(rate) * parseFloat(qty);
    $('#QtxtValue').val(Math.round(parseFloat(value)).toFixed(_decimal));
    var lvalue = parseFloat(value) * parseFloat(exrate);
   
    $('#QtxtLValue').val(Math.round(parseFloat(lvalue)).toFixed(_decimal));

}
 

function calculatevalue(index) {
    debugger;
    var rate = parsenumeric($('#QtxtRate_' + index));
    var qty = parsenumeric($('#QtxtQty_' + index));
    if (rate == '')
        rate = 0;
    if (qty == '')
        qty = 0;

    $('#QtxtRate_'+index).val(parseFloat(rate).toFixed(_decimal));
    
    if ($('#DefaultCurrencyID').val() == $('#QCurrencyID_' + index).val()) {
        $('#QtxtExRate').val(1);
        $('#QtxtLValue_' + index).val(parseFloat(value).toFixed(2));
        var value = parseFloat(rate) * parseFloat(qty);
        $('#Qspanvalue_' + index).html(value);
    }
    else {
        var exrate = $('#QtxtExRate_' + index).val();
        var value = parseFloat(rate) * parseFloat(qty);
        $('#QtxtFValue_' + index).val(parseFloat(value).toFixed(2));
        $('#Qspanvalue_' + index).html(value);
    }         
}

 
 
 

function AddDetail(obj) {
    debugger;
    let isValid = true;

    // Clear old validation states
    $(".form-control, .form-select").removeClass("is-invalid is-valid");

    function checkField(selector, message) {
        let $el = $(selector);
        if (!$el.val() || $el.val() === "Select" || $el.val().trim() === "") {
            $el.addClass("is-invalid");
            $el.next(".invalid-feedback").remove(); // remove old message
            $el.after('<div class="invalid-feedback">' + message + '</div>');
            isValid = false;
        } else {
            $el.addClass("is-valid");
            $el.next(".invalid-feedback").remove();
        }
    }

    // Validate required fields
    checkField("#QEstimationCategoryID", "Please select category");
    checkField("#drpEquipment", "Please select equipment");
    if ($('#QEstimationCategoryID').val() == 1) //equipment
    {
        checkField("#QtxtModel", "Please enter model/unit");
    }
    else {
        checkField("#QtxtModel1", "Please enter model/unit");
    }
    checkField("#QtxtQty", "Please enter valid quantity");
    checkField("#QUnitID", "Please select unit type");
    checkField("#QCurrencyID", "Please select currency");
    checkField("#QtxtRate", "Please enter valid rate");

    // Extra numeric checks
    if ($("#QtxtQty").val() && (isNaN($("#QtxtQty").val()) || $("#QtxtQty").val() <= 0)) {
        $("#QtxtQty").addClass("is-invalid");
        $("#QtxtQty").next(".invalid-feedback").remove();
        $("#QtxtQty").after('<div class="invalid-feedback">Quantity must be greater than 0</div>');
        isValid = false;
    }
    if (parsenumeric($("#QtxtRate")) <= 0) {
        $("#QtxtRate").addClass("is-invalid");
        $("#QtxtRate").next(".invalid-feedback").remove();
        $("#QtxtRate").after('<div class="invalid-feedback">Rate must be greater than 0</div>');
        isValid = false;
    }

    if (!isValid) {
        return; // stop here if invalid
    }
    var itemcount = $('#EstimationDetailTable > tbody > tr').length;
    //if ($('#QEquipmentID').val() == '') {
        
    //    Swal.fire('Data Validation', 'Enter Equipment');
    //    $('#QProductFamilyID').focus();
    //    return false;
    //}
    //else if ($('#QtxtEquipmentType').val() == '') {
    //    Swal.fire('Data Validation','Enter Equipment Type!');
    //    $('#QtxtEquipmentType').focus();
    //    return false;
    //}
    //else if ($('#QtxtModel').val() == '') {
    //    Swal.fire('Data Validation', 'Enter Model!');
    //    $('#QtxtEquipmentType').focus();
    //    return false;
    //}
    //else if ($('#QtxtQty').val() == '' || $('#QtxtQty').val() == '0') {
        
    //    Swal.fire('Data Validation', 'Enter Qty!');
    //    $('#QtxtQty').focus();
    //    return false;
    //}
    //else if ($('#QtxtRate').val() == '' || $('#QtxtRate').val() == '0') {
    //    Swal.fire('Data Validation', 'Enter Rate!');
    //    $('#QtxtRate').focus();
    //    return false;
    //}

    $(obj).attr('disabled', 'disabled');
    var workingdays = 0;
    
    if ($('#txtpaymentdays1').val() == undefined)
        workingdays = $('#PaymentDays').val();
    else
        workingdays = $('#txtpaymentdays1').val();


    var _Equipmentname = '';
    var _EquipmentId = 0;
    var _Estimationmasterid= 0;
    var _Model=''
    if ($('#QEstimationCategoryID').val() == 1) //equipment
    {
        _Equipmentname = $('#drpEquipment').select2('data')[0]?.text;
        _EquipmentId = $('#drpEquipment').val();
        _Model = $('#QtxtModel').val();
    }
    else {
        _Estimationmasterid = $('#drpEquipment').val();      
        _Equipmentname = $('#drpEquipment').select2('data')[0]?.text;
        _Model = $('#QtxtModel1').val();
    }

    var equipmententry = {
        EstimationDetailID: 0,
        EstimationID: $('#EstimationID').val(),
        CategoryName: $('#QEstimationCategoryID option:selected').text(),
        EstimationCategoryID: $('#QEstimationCategoryID').val(),
        EstimationMasterID: _Estimationmasterid,
        EquipmentID: _EquipmentId,
        Description: _Equipmentname,
        Model: _Model,
        UnitID: $('#QUnitID').val(),
        CurrencyID: $('#QCurrencyID').val(),
        ExchangeRate: $('#QtxtExRate').val(),
        CurrencyCode: $('#QCurrencyID option:selected').text(),
        UnitName: $('#QUnitID option:selected').text(),
        Qty: parsenumeric($('#QtxtQty')),
        Rate: parsenumeric($('#QtxtRate')),
        FValue: parsenumeric($('#QtxtFValue')),
        LValue: parsenumeric($('#QtxtLValue')),
        Deleted: false,
        RowType:"false"
                
    }

    var quotationdetails = [];
    for (i = 0; i < itemcount; i++) {
        var _rowtype = $('#QRowType_' + i).val();
        var _EstimationmasterId = 0;
        var _EquipmentId = 0;
        var _EstimationmasterId = 0;
        if ($('#QEstimationCategoryID_' + i).val() == 1) {
            _EquipmentId = $('#QEquipmentID_' + i).val();
        }
        else {
            _EstimationmasterId = $('#QEstimationMasterID_' + i).val();
        }
        if (_rowtype != "Total") {
            var quotationdetail = {
                EstimationDetailID: $('#QEstimationDetailID_' + i).val(),
                EstimationID: $('#EstimationID').val(),
                EstimationCategoryID: $('#QEstimationCategoryID_' + i).val(),
                EstimationMasterID: _EstimationmasterId,
                CategoryName: $('#QtxtCategoryName_' + i).val(),
                UnitName: $('#QUnitName_' + i).val(),
                UnitID: $('#QUnitID_' + i).val(),
                CurrencyID: $('#QCurrencyID_' + i).val(),
                CurrencyCode: $('#QCurrencyName_' + i).val(),
                ExchangeRate: $('#QtxtExRate_' + i).val(),
                EquipmentID: _EquipmentId,
                Description: $('#QtxtDescription_' + i).val(),
                Model: $('#QtxtModel_' + i).val(),
                Qty: $('#QtxtQty_' + i).val(),
                Rate: parsenumeric($('#QtxtRate_' + i)),
                FValue: parsenumeric($('#QtxtFValue_' + i)),
                LValue: parsenumeric($('#QtxtLValue_' + i)),
                Deleted: $('#QDeleted_' + i).val(),
                Roworder: $('#QRoworder_' + i).val(),
                RowType: $('#QRowType_' + i).val()
            }

            quotationdetails.push(quotationdetail);
        }
    }
   
    
    $.ajax({
        type: "POST",
        url: '/Estimation/AddItem/',
        datatype: "html",
        data: { invoice: equipmententry, index: -1, Details: JSON.stringify(quotationdetails), WorkingDays: workingdays },
        success: function (data) {
            $("#EstimationDetailContainer").html(data);
            $(obj).removeAttr('disabled');
            $('#QEquipmentCategory').focus()
            //calculatequotationvalue();
            clearDetail();
            //calculatelandingcost();
        }
    });
}

function checkFields() {
    let isValid = true;

    // List all mandatory fields
    const fields = [
        "#QEstimationCategoryID",
        "#drpEquipment",
        "#QtxtModel",
        "#QtxtQty",
        "#QUnitID",
        "#QCurrencyID",
        "#QtxtRate"
    ];

    fields.forEach(function (selector) {
        const $field = $(selector);
        if ($field.val() === "" || $field.val() === null) {
            $field.removeClass("is-valid").addClass("is-invalid");
            isValid = false;
        } else {
            $field.removeClass("is-invalid").addClass("is-valid");
        }
    });

    return isValid;
}

 

function SaveEstimation() {
    debugger;

    
    var emptyrow = $('#EstimationDetailTable > tbody > tr').html();
    if (emptyrow != undefined) {
        if (emptyrow.indexOf('No data available in table') >= 0) {
            $('#EstimationDetailTable > tbody').html('');
        }
    }
    var itemcount = $('#EstimationDetailTable > tbody > tr').length;

    if (itemcount == 0) {
        $('#spanerr').html('Enter Item Detail!');
        return false;
    }

    $('#btnSaveQuotation').attr('disabled', 'disabled');

    var estimation = {
        EstimationID: $('#EstimationID').val(),
        EnquiryID: $('#EnquiryID').val(),       
        EstimationNo: $('#EstimationNo').val(),
        EmployeeID: $('#EmployeeID').val(),        
        EstimationDate: $('#EstimationDate').val(),
        CurrencyID: $('#CurrencyID').val(),
        ExchangeRate: $('#ExchangeRate').val(),
        VarNo: $('#VarNo').val(),                
        TotalLCValue: parsenumeric($('#TotalLCValue')),
        TotalFCValue: parsenumeric($('#TotalFCValue')),
        Notes: $('#Notes').val(),
        PaymentDays: $('#PaymentDays').val(),
        FreeServiceDays: $('#FreeServiceDays').val(),
        ChargeableMonth: $('#ChargeableMonth').val(),
        FinChargePercent: $('#FinChargePercent').val(),
        FinCharge: $('#FinCharge').val(),
        FinPerMonthPercent: $('#FinPerMonthPercent').val(), 
        FinPerMonth: $('#FinPerMonth').val(),
        EquipmentsTotal: $('#EquipmentsTotal').val(),
        AccessoriesTotal: $('#AccessoriesTotal').val(),
        FreightTotal: $('#FreightTotal').val(),
        FinanceChargesTotal: $('#FinanceChargesTotal').val(),
        LandingCost: $('#LandingCost').val(),
        LocalChargesTotal: $('#LocalChargesTotal').val(),
        OtherChargesTotal: $('#OtherChargesTotal').val(),
        MarginPercent: $('#MarginPercent').val(),
        Margin: $('#Margin').val(),
        SellingValue: $('#SellingValue').val(),
        SoharValue: $('#SoharValue').val(),
        SoharValueOMR: $('#SoharValueOMR').val(),
        LandingCostOMR: $('#LandingCostOMR').val(),
        TotalLandingCostOMR: $('#TotalLandingCostOMR').val()
        
    }
    
    var estimationdetails = [];
    for (i = 0; i < itemcount; i++) {
        var _rowtype = $('#QRowType_'+i).val();
        var _EstimationmasterId = 0;
        var _EquipmentId = 0;
        var _EstimationmasterId = 0;
        if ($('#QEstimationCategoryID_' + i).val() == 1) {
            _EquipmentId = $('#QEquipmentID_' + i).val();
        }
        else {
            _EstimationmasterId = $('#QEstimationMasterID_' +i).val();
        }

        if (_rowtype != "Total") {

            var _detail = {
                EstimationDetailID: $('#QEstimationDetailID_' + i).val(),
                EstimationID: $('#EstimationID').val(),
                EstimationCategoryID: $('#QEstimationCategoryID_' + i).val(),
                EstimationMasterID: $('#QEstimationMasterID_' + i).val(),
                EquipmentID: _EquipmentId,
                CategoryName: $('#QtxtCategoryName_' + i).val(),
                Description: $('#QtxtDescription_' + i).val(),
                UnitID: $('#QUnitID_' + i).val(),
                CurrencyID: $('#QCurrencyID_' + i).val(),
                ExchangeRate: $('#QtxtExRate_' + i).val(),
                Model: $('#QtxtModel_' + i).val(),
                Qty: parsenumeric($('#QtxtQty_' + i)),
                Rate: parsenumeric($('#QtxtRate_' + i)),
                FValue: parsenumeric($('#QtxtFValue_' + i)),
                LValue: parsenumeric($('#QtxtLValue_' + i)),
                Deleted: $('#QDeleted').prop('checked'),
                RowType: $('#QRowType').val()
            }

            estimationdetails.push(_detail);
        }

        if (itemcount == (i + 1)) {

            $.ajax({
                type: "POST",
                url: '/Estimation/SaveEstimation/',
                datatype: "json",
                data: { estimation: estimation, Details: JSON.stringify(estimationdetails) },
                success: function (response) {
                    if (response.status == "ok") {
                        Swal.fire("Save Status!", response.message, "success");

                        setTimeout(function () {
                            window.location.href = "/Estimation/Create?id=" + response.EstimationID;
                        }, 100)

                    }
                    else {
                        $('#btnSaveQuotation').removeAttr('disabled');
                        Swal.fire("Save Status!", response.message, "warning");
                        setTimeout(function () {
                            window.location.href = "/Estimation/Create?id=" + response.EstimationID;
                        }, 100)
                    }

                }
            });
        }
    }

  
}
function showquotationdetails() {
    var quotationid = $('#QuotationID').val();
    if (quotationid == 0 || quotationid=='') {
        Swal.fire('Data Validation', 'Select Quotation No. to retrieve the Details!', 'info');
    }
    else {
        showQuotationEntry(quotationid);
    }    

}
 
function clearDetail() {
 
           
    $('#QEstimationCategoryID').val('').trigger('change');
    $('#QEquipmentID').val(0);
    $('#QtxtEquipmentType').val('');
                        
    $('#QtxtModel').val('');
    $('#QtxtModel1').val('');
    $('#QUnitID').val('1');
                                    
    $('#QtxtQty').val('');
    
    $('#QtxtRate').val('');
    $('#QtxtFValue').val('');
    $('#QtxtLValue').val('');
    $('#QtxtValue').val('');
    let newOption1 = new Option("", "", false, true);  // (text, value, defaultSelected, selected)
    $("#drpEquipment").append(newOption1).trigger('change');
    $('#QEstimationCategoryID').focus();
       
    
}
 

function DeleteEstimationEntry() {
    if ($('#SelectedQuotationId').val() != '') {
        var id = $('#SelectedQuotationId').val();
        Swal.fire({ title: "Are you sure?", text: "You won't be able to revert this!", icon: "warning", showCancelButton: !0, confirmButtonColor: "#34c38f", cancelButtonColor: "#f46a6a", confirmButtonText: "Yes, delete it!" }).then(
            function (t) {
                if (t.value) {
                    $.ajax({
                        type: "POST",
                        url: '/Quotation/DeleteQuotation',
                        datatype: "html",
                        data: {
                            'id': id
                        },
                        success: function (response) {
                            if (response.status == "ok") {
                                Swal.fire("Delete Status!", response.message, "success");
                                setTimeout(function () {
                                    window.location.reload();
                                }, 100)

                            }
                            else {
                                Swal.fire("Delete Status!", response.message, "warning");
                            }

                        }
                    });

                }
            });
    }

}
function DeleteDetailEntry(index) {
    var workingdays = 180;
    if ($('#txtpaymentdays1').val() == undefined)
        workingdays = $('#PaymentDays').val();
    else if ($('#txtpaymentdays1').val() == '') {
        $('#txtpaymentdays1').val($('#PaymentDays').val());
        workingdays = $('#PaymentDays').val();
    }
    else
        workingdays = $('#txtpaymentdays1').val();

    var itemcount = $('#EstimationDetailTable > tbody > tr').length;
    $('#QDeleted_' + index).val(true);
    $('#quodtr_' + index).addClass('hide');
    var quotationdetails = [];
    for (i = 0; i < itemcount; i++) {
        var quotationdetail = {
            EstimationDetailID: $('#QEstimationDetailID_' + i).val(),
            EstimationID: $('#EstimationID').val(),
            EstimationCategoryID: $('#QEstimationCategoryID_' + i).val(),
            EstimationMasterID: $('#QEstimationMasterID_' + i).val(),
            CategoryName: $('#QtxtCategoryName_' + i).val(),
            UnitName: $('#QUnitName_' + i).val(),
            UnitID: $('#QUnitID_' + i).val(),
            CurrencyID: $('#QCurrencyID_' + i).val(),
            CurrencyCode: $('#QCurrencyName_' + i).val(),
            EquipmentID: $('#QEquipmentID_' + i).val(),
            Description: $('#QtxtDescription_' + i).val(),
            Model: $('#QtxtModel_' + i).val(),
            Qty: $('#QtxtQty_' + i).val(),
            Rate: parsenumeric($('#QtxtRate_' + i)),
            FValue: parsenumeric($('#QtxtFValue_' + i)),
            LValue: parsenumeric($('#QtxtLValue_' + i)),
            Deleted: $('#QDeleted_' + i).val(),
            Roworder: $('#QRoworder_' + i).val(),
            RowType: $('#QRowType_' + i).val(),

        }

        quotationdetails.push(quotationdetail);

    }


    $.ajax({
        type: "POST",
        url: '/Estimation/AddItem/',
        datatype: "html",
        data: { invoice: '', index: -1, Details: JSON.stringify(quotationdetails), WorkingDays: $('#txtpaymentdays1').val() },
        success: function (data) {
            $("#EstimationDetailContainer").html(data);
            //$(obj).removeAttr('disabled');
            $('#QEquipmentCategory').focus()
            
            calculatequotationvaluenew();
            //calculatelandingcost();
        }
    });
}

function RefreshCalulation() {
    var workingdays = 180;
    if ($('#txtpaymentdays1').val() == undefined)
        workingdays = $('#PaymentDays').val();
    else if ($('#txtpaymentdays1').val() == '') {
        $('#txtpaymentdays1').val($('#PaymentDays').val());
        workingdays = $('#PaymentDays').val();
    }
    else
        workingdays = $('#txtpaymentdays1').val();

    var itemcount = $('#EstimationDetailTable > tbody > tr').length;
    
    var quotationdetails = [];
    for (i = 0; i < itemcount; i++) {
        var quotationdetail = {
            EstimationDetailID: $('#QEstimationDetailID_' + i).val(),
            EstimationID: $('#EstimationID').val(),
            EstimationCategoryID: $('#QEstimationCategoryID_' + i).val(),
            EstimationMasterID: $('#QEstimationMasterID_' + i).val(),
            CategoryName: $('#QtxtCategoryName_' + i).val(),
            UnitName: $('#QUnitName_' + i).val(),
            UnitID: $('#QUnitID_' + i).val(),
            CurrencyID: $('#QCurrencyID_' + i).val(),
            CurrencyCode: $('#QCurrencyName_' + i).val(),
            EquipmentID: $('#QEquipmentID_' + i).val(),
            Description: $('#QtxtDescription_' + i).val(),
            Model: $('#QtxtModel_' + i).val(),
            Qty: $('#QtxtQty_' + i).val(),
            Rate: parsenumeric($('#QtxtRate_' + i)),
            FValue: parsenumeric($('#QtxtFValue_' + i)),
            LValue: parsenumeric($('#QtxtLValue_' + i)),
            Deleted: $('#QDeleted_' + i).val(),
            Roworder: $('#QRoworder_' + i).val(),
            RowType: $('#QRowType_' + i).val(),

        }

        quotationdetails.push(quotationdetail);

    }


    $.ajax({
        type: "POST",
        url: '/Estimation/AddItem/',
        datatype: "html",
        data: { invoice: '', index: -1, Details: JSON.stringify(quotationdetails), WorkingDays: workingdays },
        success: function (data) {
            $("#EstimationDetailContainer").html(data);
            //$(obj).removeAttr('disabled');
            $('#QEquipmentCategory').focus()

            calculatequotationvaluenew();
            //RefreshCalulation();
            //calculatelandingcost();
        }
    });
}


function calculatequotationvaluenew() {
    debugger;

    var total = $('#TotalLandingCostOMR').val();;
        
    var _marginpercent = $('#MarginPercent').val();
    if (_marginpercent == '') {
        _marginpercent = 0;
        $('#MarginPercent').val(0);
    }
    //I29 / (1 - H30) - I29
    //Landingcost/(1-Marginpercent)-LandingCostomr
    var _marginpercent = 1 - parseFloat(_marginpercent) / 100.00;
    console.log(_marginpercent);
    var _sellingvalue = parseFloat(total) / (_marginpercent);
    var _margin = (parseFloat(_sellingvalue) - parseFloat(total));
    if (_margin == '') {
        margin = 0;
    }
    $('#Margin').val(parseFloat(Math.round(_margin)).toFixed(_decimal));
    $('#SellingValue').val(parseFloat(Math.round(_sellingvalue)).toFixed(_decimal));
    $('#spanSellingValue').html(numberWithCommas($('#SellingValue').val()));
    $('#spanMargin').html(numberWithCommas($('#Margin').val()));

    

}
function calculatequotationvalue() {
    debugger;
    var idtext = 'QtxtLValue_';
    var _EquipmentsTotal = 0;
    var _AccessoriesTotal = 0;
    var _FreightTotal = 0;
    var _FinanceChargesTotal = 0;
    var _LocalChargesTotal = 0;
    var _TransportChargesTotal = 0;
    var total = 0;
    var total1 = 0;
    $('[id^=' + idtext + ']').each(function (index, item) {
        if ($('#QDeleted_' + index).val() != 'true') {
            var _estimationcategoryid = $('#QEstimationCategoryID_' + index).val();
                    
            total = parseFloat(total) + parseFloat(parsenumeric($('#QtxtLValue_' + index)));
            if (_estimationcategoryid == 1) { //equipment
                _EquipmentsTotal = parseFloat(_EquipmentsTotal) + parseFloat(parsenumeric($('#QtxtFValue_' + index)));
            }
            else if (_estimationcategoryid == 2) { //accessories
                _AccessoriesTotal = parseFloat(_AccessoriesTotal) + parseFloat(parsenumeric($('#QtxtFValue_' + index)));
            }
            else if (_estimationcategoryid == 3) { //Freight
                _FreightTotal = parseFloat(_FreightTotal) + parseFloat(parsenumeric($('#QtxtFValue_' + index)));
            }
            else if (_estimationcategoryid == 4) { //Finance Charges
                _FinanceChargesTotal = parseFloat(_FinanceChargesTotal) + parseFloat(parsenumeric($('#QtxtFValue_' + index)));
            }
            else if (_estimationcategoryid == 5) { //Local Charges
                _LocalChargesTotal = parseFloat(_LocalChargesTotal) + parseFloat(parsenumeric($('#QtxtLValue_' + index)));
            }
            else if (_estimationcategoryid == 6) { //Transport Charges
                _TransportChargesTotal = parseFloat(_TransportChargesTotal) + parseFloat(parsenumeric($('#QtxtLValue_' + index)));
            }
        }
    });
    var _EquipmentAccessTotal = parseFloat(_EquipmentsTotal) + parseFloat(_AccessoriesTotal) + parseFloat(_FreightTotal);
    $('#EquipmentAccessTotal').val(parseFloat(_EquipmentAccessTotal).toFixed(2));
    $('#EquipmentsTotal').val(parseFloat(_EquipmentsTotal).toFixed(2));
    $('#AccessoriesTotal').val(parseFloat(_AccessoriesTotal).toFixed(2));
    $('#FreightTotal').val(parseFloat(_FreightTotal).toFixed(2));
    $('#FinanceChargesTotal').val(parseFloat(_FinanceChargesTotal).toFixed(_decimal));
    $('#LocalChargesTotal').val(parseFloat(_LocalChargesTotal).toFixed(_decimal));
    $('#TransportChargesTotal').val(parseFloat(_TransportChargesTotal).toFixed(_decimal));
    $('#TotalLCValue').val(parseFloat(total).toFixed(_decimal));
    $('#spanLCvalue').html(numberWithCommas(Math.round(total).toFixed(_decimal)));
    //alert(total);
    var _marginpercent = $('#MarginPercent').val();
    if (_marginpercent == '') {
        _marginpercent = 0;
        $('#MarginPercent').val(0);
    }
    
    var _marginpercent = 100 - parseFloat(_marginpercent);
    var _sellingvalue = parseFloat(total) / (_marginpercent / 100.00);
    var _margin = (parseFloat(_sellingvalue) - parseFloat(total));
    if (_margin == '') {
        margin = 0;
    }
    $('#Margin').val(parseFloat(Math.round(_margin)).toFixed(_decimal));
    $('#SellingValue').val(parseFloat(Math.round(_sellingvalue)).toFixed(_decimal));
    $('#spanSellingValue').html(numberWithCommas($('#SellingValue').val()));
    $('#spanMargin').html(numberWithCommas($('#Margin').val()));
    
    idtext = 'QtxtFValue_'
    total = 0;
    $('[id^=' + idtext + ']').each(function (index, item) {
        if ($('#QDeleted_' + index).val() != 'true') {
            total = parseFloat(total) + parseFloat(parsenumeric($('#QtxtFValue_' + index)));
        }
    });
    $('#TotalFCValue').val(parseFloat(total).toFixed(2));
    $('#spanFCvalue').html(numberWithCommas(Math.round(total).toFixed(2)));
    

}
function showquotationprint() {
    debugger;
    if ($('#SelectedQuotationId').val() != '') {
        console.log($('#JobID').val());
        console.log($('#SelectedQuotationId').val());
        $('#aqprint').attr('href', '/Job/ReportPrint?id=' + $('#JobID').val() + '&option=6&quotationid=' + $('#SelectedQuotationId').val());
        $('#aqprint').trigger('click');
    }
}
function newQuotation() {

    $.ajax({
        type: "POST",
        url: '/Quotation/ShowQuotationEntry/',
        //datatype: "html",
        data: { Id: QuotationId, EnquiryID: $('#EnquiryID').val(), QuotationID: QuotationId },
        success: function (response) {
            console.log(response.data);
            var data = response.data;
            var myDate = new Date(data.QuotationDate.match(/\d+/)[0] * 1);
            var cmon = myDate.getMonth() + 1;
            var entrydate = myDate.getDate() + "/" + cmon + "/" + myDate.getFullYear();

            $('#QuotationID').val(0);
            $('#QuotationNo').val(data.QuotationNo);
            $('#QuotationDate').val(entrydate);
            $('#QuotationDate').val(entrydate).trigger('change');
            $('#QCurrencyId').val(data.CurrencyId).trigger('change');
            $('#Validity').attr('svalue', data.Validity);

            $('#Version').val(data.Version);
            $('#QuotationContact').val(data.ContactPerson);
            $('#QuotationMobile').val(data.MobileNumber);
            $('#QuotationPayment').val(data.PaymentTerms)
            $('#QtxtTerms').val(data.TermsandConditions);
            $('#QtxtSalutation').val(data.Salutation);
            $('#QuotationValue').val(data.QuotationValue);
            $('#EnquiryID').val(data.EnquiryID);
            $('#QtxtSubject').val(data.SubjectText);
            $('#ClientID').val(data.ClientID);
            $('#ClientDetail').val(data.ClientDetail);
            $('#btnSaveQuotation').html('Add & Save');
            //$("#QuotationEntryContainer").html(data);                       
            $.ajax({
                type: "POST",
                url: '/Quotation/ShowQuotationDetailList/',
                datatype: "html",
                data: { QuotationId: QuotationId },
                success: function (data1) {
                    $("#QuotationDetailContainer").html(data1);
                    if (QuotationId > 0) {

                        //$('#btnSaveQuotation').html('Update');         
                        setTimeout(function () {
                            $('#Validity').val($('#Validity').attr('svalue')).trigger('change');
                        }, 100)

                    }

                }
            });
        }
    });
}
(function ($) {

    'use strict';
    function initformControl() {

       
        $('#EstimationDate').datepicker({
            dateFormat: 'dd-mm-yy',
            changeMonth: true,
            changeYear: true
        }).on('changeDate', function (e) {
            $(this).datepicker('hide');
        });

      
        $('#QEstimationCategoryID').change(function () {
            let newOption1 = new Option("", "", false, true);  // (text, value, defaultSelected, selected)
            $("#drpEquipment").append(newOption1).trigger('change');

            if ($('#QEstimationCategoryID').val() == 1) {
                $('#QEquipmentID').removeClass('d-xl-none');
                $('#QtxtModel').removeClass('d-xl-none');
               // $('#QtxtEquipmentType').addClass('d-xl-none');
                $('#QtxtModel1').addClass('d-xl-none');
                $('#lblQty').html('Quantity');
            }
            else if ($('#QEstimationCategoryID').val() == 5 || $('#QEstimationCategoryID').val()==6) { //autocalc items
                $('#QEquipmentID').addClass('d-xl-none');
               // $('#QtxtEquipmentType').removeClass('d-xl-none');
                $('#QtxtModel').addClass('d-xl-none');
                $('#QCurrencyID').val($('#DefaultCurrencyID').val());
                $('#QCurrencyID').trigger('change');
                $('#QtxtModel1').removeClass('d-xl-none');
                $('#QtxtRate').val($('#SoharValueOMR').val());
                $('#QtxtQty').val(1);
                $('#QtxtQty').val(1);
                $('#lblQty').html('%');
                calculatevalue1();
            }
            else {
                $('#QEquipmentID').addClass('d-xl-none');
                $('#QtxtEquipmentType').removeClass('d-xl-none');
                $('#QtxtModel').addClass('d-xl-none');
                $('#QtxtModel1').removeClass('d-xl-none');
                $('#lblQty').html('Quantity');
                
            }

        })

        if ($('#hdnQuotationMode').val() == 'New') {
            $('#QuotationNo').css('color', 'blue');
        }
        
        $('#MarginPercent').change(function () {
            calculatequotationvaluenew();
        })

        $('#EnquiryID').change(function () {
            if ($('#QuotationID').val() == 0) { //new mode

                //GetNewQuotationNo
                $.ajax({
                    url: "/Quotation/GetNewQuotationNo",
                    data: { EnquiryID: $('#EnquiryID').val(), EmployeeID: $('#QEngineerID').val() },
                    dataType: "json",
                    type: "GET",
                    success: function (response) {
                       
                        $('#QuotationNo').val(response.QuotationNo);
                        $('#QuotationNo').attr('readonly', 'readonly');
                        $('#Version').val(response.Version);
                        $('#Version').attr('readonly', 'readonly');
                        $('#ClientID').val(response.ClientID);
                        $('#ClientDetail').val(response.ClientDetail);
                        $('#ContactPerson').val(response.ContactPerson);
                        $('#MobileNumber').val(response.MobileNumber);
                        $('#QuotationNo').css('color', 'blue');
                    }
                });
            }

        })
      
        $("#QtxtModel").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/Enquiry/GetModel',
                    datatype: "json",
                    data: {
                        term: request.term
                    },
                    success: function (data) {
                        response($.map(data, function (val, item) {
                            return {
                                label: val.ModelName,
                                value: val.ID


                            }
                        }))
                    }
                })
            },
            minLength: 1,
            autoFocus: false,
            focus: function (e, ui) {
                e.preventDefault();
                $('#QtxtModel').val(ui.item.label);


            },
            select: function (e, ui) {
                e.preventDefault();
                $('#QtxtModel').val(ui.item.label);

            },

        });

        
    
 
     
 
        
    }
    function init() {
        initformControl();
    }
    $(document).ready(function () {
        init();
       
    })

})(jQuery);


(function () {
    "use strict";
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

                            SaveEstimation();

                        }
                    },
                    !1
                );
            });
        },
        !1
    );
})(),
    $(document).ready(function () {
        var suppressOpenOnClear = false;
        $('#CurrencyID').change(function () {
            debugger;
            $('#thfvalue').html($('#CurrencyID option:selected').text());
        })
        $('#QCurrencyID').change(function () {
            $.ajax({
                url: "/Currency/GetExchangeRate",
                data: { CurrencyID: $('#QCurrencyID').val() },
                dataType: "json",
                type: "GET",
                success: function (data) {
                    console.log(data);
                    $('#QtxtExRate').val(data);
                    $('#lblExRate').html(data);
                }
            });
        })
        if ($('#EstimationID').val() > 0) {
            setTimeout(function () {
                
                ApplyMargin();
            }, 200)

        }
        $('#QEstimationCategoryID').blur(function () {
            $('#drpEquipment').select2('open');
        })
        $('#QEstimationCategoryID').val(1).trigger('change');

        $('.equipment-select').each(function () {
            var $select = $(this);

            // Handle the clear event
            $select.on('select2:select2-selection__arrow', function () {
                suppressOpenOnClear = true;

                // Remove focus from both the hidden <select> and visible Select2 box
                setTimeout(() => {
                    $select.blur();
                    $select.next('.select2-container').find('.select2-selection').blur();
                }, 0);
            });

            // Prevent dropdown from reopening immediately after clear
            $select.on('select2:opening', function (e) {
                if (suppressOpenOnClear) {
                    e.preventDefault(); // Stop it from opening
                    suppressOpenOnClear = false;
                }
            });
            var defaultOption = new Option($select.attr('boxname'), $select.attr('boxid'), true, true);
            $select.append(defaultOption);
            //// Append it to the select
            $select.append('<option id="" value=""></option>').trigger('change');
            $select.select2({
                placeholder: 'Select a product',
                allowClear: false,
                width:"100%",
                minLength: 1,
                ajax: {
                    url: '/Estimation/GetEquipmentType',
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        // var category = $select.closest('tr').find('.category').val();
                        return {
                            term: params.term,
                            EnquiryID: $('#EnquiryID').val(),
                            CategoryID: $('#QEstimationCategoryID').val()
                        };
                    },
                    processResults: function (data) {
                        return {
                            results: data.map(function (item) {
                                return { id: item.ID, text: item.Text };
                            })
                        };
                    },
                    cache: true
                }
            });


            $select.change(function () {
                var categoryId = $('#QEstimationCategoryID').val();
                var _EstimationType = $(this).select2('data')[0]?.text; //$("#BoxName").val(); $(this).val();

                if (categoryId != 1) {

                    $.ajax({
                        url: '/Estimation/GetEstimationTypeByText',
                        datatype: "json",
                        data: {
                            TypeName: _EstimationType
                        },
                        success: function (data) {
                            if (data!=null) {                                
                                $('#QtxtQty').val(data.Qty);
                                $('#QUnitID').val(data.UnitID);
                                $('#QCurrencyID').val(data.CurrencyID).trigger('change');
                                $('#QtxtModel1').val(data.Remarks);
                            
                                calculatevalue1();
                            }
                            else {
                                
                            }

                        }

                    })
                }
            });
        });


        document.addEventListener('keydown', function (e) {
            if (e.target.tagName === 'INPUT' && (e.key === "ArrowDown" || e.key === "ArrowUp")) {
                const activeElement = e.target;
                const currentRow = activeElement.closest('tr');
                let targetRow = null;

                if (e.key === "ArrowDown") {
                    targetRow = currentRow?.nextElementSibling;
                } else if (e.key === "ArrowUp") {
                    targetRow = currentRow?.previousElementSibling;
                }

                if (targetRow) {
                    // Try to focus the input in the same column index
                    const inputs = Array.from(currentRow.querySelectorAll('input'));
                    const index = inputs.indexOf(activeElement);
                    const targetInputs = targetRow.querySelectorAll('input');

                    if (targetInputs[index]) {
                        targetInputs[index].focus();
                        e.preventDefault();
                    }
                }
            }
        });
    });
