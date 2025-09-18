var _decimal = $('#hdncompanydecimal').val();
function toggleDropdown() {
    var content = document.getElementById("drptxtTerms");
    content.style.display = content.style.display === "block" ? "none" : "block";
}
 
function calculatevalue1() {
    debugger;
    var rate = $('#QtxtRate').val();
    var qty = $('#QtxtQty').val();
    if (rate == '')
        rate = 0;
    if (qty == '')
        qty = 0;
    $('#QtxtRate').val(parseFloat(rate).toFixed(_decimal));

    var value = parseFloat(rate) * parseFloat(qty);
    $('#QtxtValue').val(parseFloat(value).toFixed(2));

}

//based on value rate calculate
function calculatevalue2() {
    debugger;
    var value = $('#QtxtAmount').val();
    var qty = $('#QtxtQty').val();
    var rate = $('#QtxtRate').val();

    if (value == '')
        valu = 0;
    if (qty == '')
        qty = 0;
    if (parseFloat(value) > 0 && parseFloat(qty) > 0) {
        rate = parseFloat(value) / parseFloat(qty);
    }
    $('#QtxtUnitRate').val(parseFloat(rate).toFixed(_decimal));

    //var value = parseFloat(rate) * parseFloat(qty);
    //$('#QtxtValue').val(parseFloat(value).toFixed(2));

}

function calculatevalue(index) {
    var rate = parsenumeric($('#QtxtUnitRate_' + index));
    var qty = parsenumeric($('#QtxtQty_' + index));
    if (rate == '')
        rate = 0;
    if (qty == '')
        qty = 0;

    $('#QtxtUnitRate').val(parseFloat(rate).toFixed(_decimal));
    var value = parseFloat(rate) * parseFloat(qty);
    $('#QtxtAmount_' + index).val(parseFloat(value).toFixed(2));
    $('#QspanAmount_' + index).html(parseFloat(value).toFixed(2));
    calculatequotationvalue();
}

function calculatevalue3(index) {
    var value = $('#QtxtValue_' + index).val();
    var qty = $('#QtxtQty_' + index).val();
    if (value == '')
        value = 0;
    if (qty == '')
        qty = 0;

    if (parseFloat(value) > 0 && parseFloat(qty) > 0) {

        var rate = parseFloat(value) / parseFloat(qty);
        $('#QtxtRate_' + index).val(parseFloat(rate).toFixed(_decimal));
        //$('#QtxtValue_' + index).val(parseFloat(value).toFixed(2));
        calculatequotationvalue();
    }
    else {
        $('#QtxtRate_' + index).val(0);
        calculatequotationvalue();
    }
}
function checkdeletedentry() {
    var idtext = 'quodtr_'
    $('[id^=' + idtext + ']').each(function (index, item) {
        if ($('#QDeleted_' + index).val() == 'True') {
            $('#quodtr_' + index).addClass('hide');
        }
        else {
            $('#quodtr_' + index).removeClass('hide');
        }
    });

}
function setUOMcombo() {
    var idtext = 'QUOM_'
    $('[id^=' + idtext + ']').each(function (index, item) {
        $('#QUOM_' + index).val($('#QUOM_' + index).attr('value')).trigger('change');
    });
}
function setquotationowactive(index1) {
    var idtext = 'quotr_'
    $('[id^=' + idtext + ']').each(function (index, item) {
        $('#quotr_' + index).removeClass('rowactive');
    });
    $('#quotr_' + index1).addClass('rowactive');
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

//add direct quotation item 
function AddDetail(obj) {
    debugger;
    let isValid = true;

    // helper to set validation state
    function setValidation(control, message) {
        let $control = $(control);

        if (message) {
            isValid = false;
            $control.addClass("is-invalid");

            // if feedback not exists, add it
            if ($control.next(".invalid-feedback").length === 0) {
                $control.after('<div class="invalid-feedback">' + message + '</div>');
            } else {
                $control.next(".invalid-feedback").text(message);  
            }
        } else {
            $control.removeClass("is-invalid");
            $control.next(".invalid-feedback").remove();
        }
    }

    // Category
    setValidation("#QEstimationCategoryID", $('#QEstimationCategoryID').val() ? "" : "Select Category");

    // Equipment
    setValidation("#drpEquipment", $('#drpEquipment').val() ? "" : "Select Equipment");

    // Model
    setValidation("#QtxtModel", $('#QtxtModel').val().trim() ? "" : "Enter Model/Unit");

    // Qty
    let qty = $('#QtxtQty').val();
    setValidation("#QtxtQty", (qty && !isNaN(qty) && parseFloat(qty) > 0) ? "" : "Enter valid Qty");

    // Unit Type
    setValidation("#QUnitID", $('#QUnitID').val() ? "" : "Select Unit Type");

    // Rate
    let rate = $('#QtxtRate').val();
    setValidation("#QtxtRate", (parsenumeric(rate) <= 0) ? "" : "Enter valid Rate");

    // Stop if not valid
    if (!isValid) {
        return false;
    }

    var itemcount = $('#QuotationDetailTables > tbody > tr').length;
    
    $(obj).attr('disabled', 'disabled');
   

    var _Equipmentname = '';
    var _EquipmentId = 0;
    var _Estimationmasterid = 0;
    var _Model = ''
    if ($('#QEstimationCategoryID').val() == 1) //equipment
    {

        _Equipmentname = $('#drpEquipment').select2('data')[0]?.text;
        _EquipmentId = $('#drpEquipment').val();
        _Model = $('#QtxtModel').val()
    }
    else {
        _Estimationmasterid = $('#drpEquipment').val();

        _Equipmentname = $('#drpEquipment').select2('data')[0]?.text;
        _Model = $('#QtxtModel1').val()
    }
    //if ($('#QUnitID').val() == '0' || $('#QUnitID').val() == '') {
    //    Swal.fire('Data Validation', 'Select Unit!', 'warning');
    //    $(obj).removeAttr('disabled');
    //    return;
    //}
    //if ($('#QUnitID').val() == '0' || $('#QUnitID').val() == '') {
    //    Swal.fire('Data Validation', 'Select Unit!', 'warning');
    //    $(obj).removeAttr('disabled');
    //    return;
    //}
    var equipmententry = {
        QuotationDetailID: 0,
        EstimationDetailID: 0,
        EstimationID: 0,
        CategoryName: $('#QEstimationCategoryID option:selected').text(),
        EstimationCategoryID: $('#QEstimationCategoryID').val(),
        EstimationMasterID: _Estimationmasterid,
        EquipmentID: _EquipmentId,
        Description: _Equipmentname,
        Model: _Model,
        UnitID: $('#QUnitID').val(),                
        UnitName: $('#QUnitID option:selected').text(),
        Quantity: parsenumeric($('#QtxtQty')),
        UnitRate: parsenumeric($('#QtxtRate')),        
        Amount: parsenumeric($('#QtxtValue')),
        Deleted: false,
        RowType: "false"

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
        
        if (_rowtype != "Total" && $('#QtxtDescription_' + i).val()!=undefined) {
            var quotationdetail = {
                EstimationDetailID: $('#QEstimationDetailID_' + i).val(),
                EstimationID: $('#EstimationID').val(),
                EstimationCategoryID: $('#QEstimationCategoryID_' + i).val(),
                EstimationMasterID: _EstimationmasterId,
                EstimationID: $('#QEstimationID_'+i).val(),
                EstimationDetailID: $('#QEstimationDetailID_' + i).val(),
                EstimationNo: $('#QEstimationNo_' +i).val(),
                CategoryName: $('#QtxtCategoryName_' + i).val(),
                UnitName: $('#QUnitName_' + i).val(),
                UnitID: $('#QUnitID_' + i).val(),                 
                EquipmentID: _EquipmentId,
                Description: $('#QtxtDescription_' + i).val(),
                Model: $('#QtxtModel_' + i).val(),
                Quantity: $('#QtxtQty_' + i).val(),
                UnitRate: parsenumeric($('#QtxtUnitRate_' + i)),
                Amount: parsenumeric($('#QtxtAmount_' + i)),
                Deleted: $('#QDeleted_' + i).val(),
                Roworder: $('#QRoworder_' + i).val()                
            }

            quotationdetails.push(quotationdetail);
        }
    }


    $.ajax({
        type: "POST",
        url: '/Quotation/AddItem/',
        datatype: "html",
        data: { invoice: equipmententry, index: -1, Details: JSON.stringify(quotationdetails), EstimationID:0 },
        success: function (data) {
            $("#QuotationDetailContainer").html(data);
            $(obj).removeAttr('disabled');
            $('#QEquipmentCategory').focus()
            calculatequotationvalue();
            clearDetail();
            $('#DirectQuotation').attr('disabled', 'disabled');
            //calculatelandingcost();
        }
    });
}
function SaveEstimationDetailEntry() {
    debugger;
     var itemcount = $('#EstimationDetailTable > tbody > tr').length;
     
    $('#btnAddEstimationDetail').attr('disabled', 'disabled');
    
        var quotationdetails = [];
        for (i = 0; i < itemcount; i++) {
             
            var _EstimationmasterId = 0;
            var _EquipmentId = 0;
            var _EstimationmasterId = 0;
            if ($('#eEstimationCategoryID_' + i).val() == 1) {
                _EquipmentId = $('#eEquipmentID_' + i).val();
            }
            else {
                _EstimationmasterId = $('#eEstimationMasterID_' + i).val();
            }
          
                var _estimationNo = $('#estimationdetailpopup').attr('EstimationNo');
                var quotationdetail = {
                    EstimationDetailID: $('#eEstimationDetailID_' + i).val(),
                    EstimationID: $('#eEstimationID_' + i).val(),
                    EstimationNo: $('#eEstimationNo_' + i).val(),
                    EstimationCategoryID: $('#eEstimationCategoryID_' + i).val(),
                    EstimationMasterID: _EstimationmasterId,
                    CategoryName: $('#etxtCategoryName_' + i).val(),
                    UnitName: $('#eUnitName_' + i).val(),
                    UnitID: $('#eUnitID_' + i).val(),
                    EquipmentID: _EquipmentId,
                    Description: $('#etxtDescription_' + i).val(),
                    Model: $('#etxtModel_' + i).val(),
                    Quantity: $('#etxtQty_' + i).val(),
                    UnitRate: parsenumeric($('#etxtRate_' + i)),
                    Amount: parsenumeric($('#etxtLValue_' + i)),
                    Checked: $('#chkEstimationItem_' + i).prop('checked')
                    // Roworder: $('#QRoworder_' + i).val(),
                    // RowType: $('#QRowType_' + i).val()
                }
            
                quotationdetails.push(quotationdetail);
            
        }

   
        $.ajax({
            type: "POST",
            url: '/Quotation/AddEstimationItem/',
            datatype: "html",
            data: { Details: JSON.stringify(quotationdetails) },
            success: function (response) {
                debugger;
                var quotationdetails = [];
                var itemcount1 = $('#QuotationDetailTables > tbody > tr').length;
                if (itemcount1 == 0) {
                    $.ajax({
                        type: "POST",
                        url: '/Quotation/AddItem1/',
                        datatype: "html",
                        data: { Details: JSON.stringify(response.data), Details1: JSON.stringify(quotationdetails), EstimationID: 0 },
                        success: function (data1) {
                            $("#QuotationDetailContainer").html(data1);
                            $('#btnAddEstimationDetail').removeAttr('disabled');
                            $('#QEquipmentCategory').focus()
                            calculatequotationvalue();
                            clearDetail();                            
                            $('#estimationdetailpopup').modal('hide');
                        }
                    });
                } else {
                    for (k = 0; k < itemcount1; k++) {

                        var _EstimationmasterId = 0;
                        var _EquipmentId = 0;
                        var _EstimationmasterId = 0;
                        if ($('#QEstimationCategoryID_' + k).val() == 1) {
                            _EquipmentId = $('#QEquipmentID_' + k).val();
                        }
                        else {
                            _EstimationmasterId = $('#QEstimationMasterID_' + k).val();
                        }
                        var _rowtype = $('#QRowType_' + i).val();
                        if (_rowtype != "Total" && $('#QtxtDescription_' + i).val() != undefined) {
                            var quotationdetail = {
                                EstimationDetailID: $('#QEstimationDetailID_' + k).val(),
                                EstimationID: $('#QEstimationID_' + k).val(),
                                EstimationNo: $('#QEstimationNo_' + k).val(),
                                EstimationCategoryID: $('#QEstimationCategoryID_' + k).val(),
                                EstimationMasterID: _EstimationmasterId,
                                CategoryName: $('#QtxtCategoryName_' + k).val(),
                                UnitName: $('#QUnitName_' + k).val(),
                                UnitID: $('#QUnitID_' + k).val(),
                                EquipmentID: _EquipmentId,
                                Description: $('#QtxtDescription_' + k).val(),
                                Model: $('#QtxtModel_' + k).val(),
                                Quantity: $('#QtxtQty_' + k).val(),
                                UnitRate: parsenumeric($('#QtxtUnitRate_' + k)),
                                Amount: parsenumeric($('#QtxtAmount_' + k)),
                                Deleted: $('#QDeleted_' + k).val(),
                                Roworder: $('#QRoworder_' + k).val()
                            }

                            quotationdetails.push(quotationdetail);
                        }

                        if ((k + 1) == itemcount1) {
                            $.ajax({
                                type: "POST",
                                url: '/Quotation/AddItem1/',
                                datatype: "html",
                                data: { Details: JSON.stringify(response.data), Details1: JSON.stringify(quotationdetails), EstimationID: 0 },
                                success: function (data1) {
                                    $("#QuotationDetailContainer").html(data1);
                                    $('#btnAddEstimationDetail').removeAttr('disabled');
                                    $('#QEquipmentCategory').focus()
                                    calculatequotationvalue();
                                    clearDetail();                                    
                                    $('#estimationdetailpopup').modal('hide');
                                }
                            });

                        }

                    }

                }
              
                
            }
        });
  
}
function SaveQuotation() {
    debugger;
    var emptyrow = $('#QuotationDetailTables > tbody > tr').html();
    if (emptyrow != undefined) {
        if (emptyrow.indexOf('No data available in table') >= 0) {
            $('#QuotationDetailTables > tbody').html('');
        }
    }
    var itemcount = $('#QuotationDetailTables > tbody > tr').length;

    
    if (itemcount == 0) {
        $('#spanerr').html('Enter Quotation Item Detail!');
        return false;
    }

    var entityitems = $('#drpQuotationTo').val();
    if (entityitems == null) {
        Swal.fire('Data Validation', 'Select Quotation To', 'warning');
        return false;
    }
    var entitys = '';
    $.each(entityitems, function (index, item) {
        if (item != '') {
            entitys = entitys + ',' + item;
        }
        $('#QuotationTo').val(entitys);
    });

    //var selectedItems = '';
    //$('.item-checkbox:checked').each(function (index,item) {
    //    //selectedItems.push($(this).val());
    //    selectedItems = selectedItems + $(item).attr('value') + ',';
    //});
    //$('#QtxtTerms').val(selectedItems);
    
    

    //var content = '';// tinymce.get('elm1').getContent()
    //var content1 = '';//tinymce.get('elm2').getContent()
    //var content2 = '';//tinymce.get('elm3').getContent()
    //var scopeofworkjsonData = JSON.stringify({ ScopeofWork: encodeURIComponent(content), Warranty: encodeURIComponent(content1), Exclusions: encodeURIComponent(content2) });


    $('#btnSaveQuotation').attr('disabled', 'disabled');

    var quotation = {
        EnquiryID: $('#EnquiryID').val(),
        QuotationID: $('#QuotationID').val(),
        QuotationNo: $('#QuotationNo').val(),
        EngineerID: $('#QEngineerID').val(),        
        QuotationDate: $('#QuotationDate').val(),
        CurrencyId: $('#QCurrencyId').val(),
        Validity: $('#Validity').val(),
        Version: $('#Version').val(),
        //ContactPerson: $('#QuotationContact').val(),
        //MobileNumber: $('#QuotationMobile').val(),
        PaymentTerms: $('#QuotationPayment').val(),
        TermsandConditions: $('#QtxtTermsCondition').val(),
        Salutation: $('#QtxtSalutation').val(),
        QuotationValue: $('#QuotationValue').val(),
        GrossAmount: $('#GrossAmount').val(),
        MarginPercent: $('#MarginPercent').val(),
        Margin: $('#Margin').val(),
        SellingValue: $('#SellingValue').val(),
        JobID: $('#JobID').val(),
        SubjectText: $('#QtxtSubject').val(),
        ClientDetail: $('#ClientDetail').val(),
        ClientID: $('#ClientID').val(),
        QuotationTo: $('#QuotationTo').val(),
        QuotationStatusID: $('#QuotationStatusID').val(),
        DiscountPercent: $('#DiscountPercent').val(),
        DiscountAmount: $('#DiscountAmount').val(),
        VATPercent: $('#VATPercent').val(),
        VATAmount: $('#VATAmount').val(),
        DeliveryTerms: $('#QtxtDeliveryTerms').val(),
        Validity: $('#QtxtValidity').val(),
        ProjectRef: $('#ProjectRef').val(),
        DirectQuotation: $('#DirectQuotation').prop('checked')
    }
    var quotationdetails = [];
    for (i = 0; i < itemcount; i++) {
        var quotationdetail = {
            ID: $('#QDetailID_' + i).val(),
            EnquiryID: $('#EnquiryID').val(),
            QuotationID: $('#QuotationID').val(),
            EquipmentID: $('#QEquipmentID_' + i).val(),
            EstimationID: $('#QEstimationID_' + i).val(), 
            EstimationCategoryID: $('#QEstimationCategoryID_' + i).val(),
            EstimationMasterID: $('#QEstimationMasterID_' + i).val(),
            EstimationDetailID: $('#QEstimationDetailID_' + i).val(),
            Description: $('#QtxtDescription_' + i).val(),

            Model: $('#QtxtModel_' + i).val(),
            UnitID: $('#QUnitID_' + i).val(),
            Quantity: parsenumeric($('#QtxtQty_' + i)),
            UnitRate: parsenumeric($('#QtxtUnitRate_' + i)),
            Amount: parsenumeric($('#QtxtAmount_' + i)),
            EquipmentStatusID: 1, // $('#eEquipmentStatusID').val(),
            Remarks: $('#QtxtRemarks_' + i).val(),
            Deleted: $('#QDeleted_' + i).val()
            //NominalCapacity: $('#QtxtNominal_' + i).val(),
            //Refrigerant: $('#QRefrigerant_' + i).val(),
            //EfficientType: $('#QtxtEfficientType_' + i).val()

    }

        quotationdetails.push(quotationdetail);


        if (itemcount==(i+1)) {
            $.ajax({
                type: "POST",
                url: '/Quotation/SaveQuotation/',
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                //data: { quotation: quotation, Details: JSON.stringify(quotationdetails) },
                data:  JSON.stringify({ quotation: quotation, Details: quotationdetails }),
                success: function (response) {
                    if (response.status == "ok") {
                        Swal.fire("Save Status!", response.message, "success");
                        $('#btnSaveQuotation').removeAttr('disabled');
                        setTimeout(function () {
                            window.location.href = "/Quotation/Create?id=" + response.QuotationId;
                        }, 100)

                    }
                    else {
                        $('#btnSaveQuotation').removeAttr('disabled');
                        Swal.fire("Save Status!", response.message, "warning");
                        //setTimeout(function () {
                        //    window.location.href = "/Quotation/Index";
                        //}, 100)
                    }

                },
                error: function (xhr, status, error) {
                    debugger;
                    $('#btnSaveQuotation').removeAttr('disabled');
                    Swal.fire("Error", "Something went wrong: " + error, "error");
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
function showcopyquotationdetails() {
    var quotationid = $('#QuotationID').val();
    if (quotationid == 0 || quotationid == '') {
        Swal.fire('Data Validation', 'Select Quotation No. to retrieve the Details!', 'info');
    }
    else {
        showQuotationEntry(quotationid);
    }
    

}
//function showQuotationEntry(QuotationId, index1) {
//    //if (QuotationId > 0)
//    //    setquotationowactive(index1);
//    $.ajax({
//        type: "POST",
//        url: '/Quotation/ShowQuotationEntry/',
//        //datatype: "html",
//        data: { Id: QuotationId, EnquiryID: $('#EnquiryID').val(), QuotationID: 0, EmployeeID: $('#QEngineerID').val()},
//        success: function (response) {
//            console.log(response.data);
//            var data = response.data;
//            var myDate = new Date(data.QuotationDate.match(/\d+/)[0] * 1);
//            var cmon = myDate.getMonth() + 1;
//            var entrydate = myDate.getDate() + "/" + cmon + "/" + myDate.getFullYear();

//            $('#QuotationID').val(data.QuotationID);
//            $('#QuotationNo').val(data.QuotationNo);
//            $('#QuotationDate').val(entrydate);
//            $('#QuotationDate').val(entrydate).trigger('change');
//            $('#QCurrencyId').val(data.CurrencyId).trigger('change');
//            $('#Validity').attr('svalue', data.Validity);

//            $('#Version').val(data.Version);
//            $('#QuotationContact').val(data.ContactPerson);
//            $('#QuotationMobile').val(data.MobileNumber);
//            $('#QuotationPayment').val(data.PaymentTerms)
//            $('#QtxtTerms').val(data.TermsandConditions);
//            $('#QtxtSalutation').val(data.Salutation);
//            $('#QuotationValue').val(data.QuotationValue);
//            $('#JobID').val(data.JobID);
//            $('#QtxtSubject').val(data.SubjectText);
//            $('#ClientDetail').val(data.ClientDetail);
//            $('#ClientID').val(data.ClientID);            
//            tinymce.get('elm1').setContent(decodeURIComponent(data.ScopeofWork));
//            tinymce.get('elm2').setContent(decodeURIComponent(data.Warranty));
//            tinymce.get('elm3').setContent(decodeURIComponent(data.Exclusions));
//            //$("#QuotationEntryContainer").html(data);                       
//            $.ajax({
//                type: "POST",
//                url: '/Quotation/ShowQuotationDetailList/',
//                datatype: "html",
//                data: { EnquiryID :$('#EnquiryID').val(), QuotationId: QuotationId },
//                success: function (data1) {
//                    $("#QuotationDetailContainer").html(data1);
//                    if (QuotationId > 0) {

//                        //$('#btnSaveQuotation').html('Update');         
//                        setTimeout(function () {
//                            $('#Validity').val($('#Validity').attr('svalue')).trigger('change');
//                        }, 100)

//                    }

//                }
//            });
//        }
//    });

//}
function clearQuotationDetail() {
    $('#QProductFamilyID').val('').trigger('change');;
    $('#QtxtEquipmentType').val('');
    $('#QEquipmentTypeID').val('0');
    $('#QtxtBrand').val('');
    $('#QtxtModel').val('');
   
    $('#QtxtQty').val(1);
    $('#QtxtRate').val(0);
    $('#QtxtValue').val('');
    $('#chkEfficientType').prop('checked', false);
    $('#QtxtEfficientType').val('');
    $('#QtxtEfficientType').val('')

    $('#chkNominalCapacity').prop('checked', false);
    $('#QtxtNominal').val('');
    $('#chkRefrigerant').prop('checked', false);

   

    var idtext = 'quotr_';
    $('[id^=' + idtext + ']').each(function (index, item) {
        $('#boetr_' + index).removeClass('rowactive');
    });
    $('#btnSaveQuotation').html('Add & Save');
}
function clearQuotation() {
    $('#QuotationContact').val('');
    $('#QuotationNo').val('');
    $('#QuotationDate').val('');
    $('#QuotationMobile').val('');
    $('#QuotationPayment').val('');
    $('#QCurrencyId').val(0).trigger('change');
    $('#QuotationValue').val('');
    $('#QtxtTerms').val('');
    $('#QtxtSalutation').val('');
    $('#QtxtSubject').val('');
    $('#ClientDetail').val('');
    //$('#Version').val($('#Version').attr('NewVersion'));
    var idtext = 'quotr_'
    $('[id^=' + idtext + ']').each(function (index, item) {
        $('#quotr_' + index).removeClass('rowactive');
    });
    showQuotationEntry(0);
    $('#btnSaveQuotation').html('Add & Save');
}

function DeleteQuotationEntry() {
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
function DeleteQuotationDetailEntry(index) {
    $('#QDeleted_' + index).val(true);
    $('#quodtr_' + index).addClass('hide');
    calculatequotationvalue();
}

function calculatequotationvalue() {
    debugger;

    let total = 0;
    const idtext = 'QtxtAmount_';

    // 1. Calculate Gross Total (skip deleted rows)
    $('[id^=' + idtext + ']').each(function (index) {
        if ($('#QDeleted_' + index).val() !== 'true') {
            total += parsenumeric($('#' + idtext + index));
        }
    });

    // Update Gross Amount fields
    $('#GrossAmount').val(total.toFixed(3));
    $('#QspanGrossAmount').html(numberWithCommas(total.toFixed(3)));

    // 2. If Margin > 0, use SellingValue instead of Gross
    const margin = parseFloat($('#Margin').val() || 0);
    if (margin > 0) {
        total = parseFloat($('#SellingValue').val() || total);
    }

    // 3. Get Discount & VAT inputs
    const discountPercent = parseFloat($('#DiscountPercent').val() || 0);
    const vatPercent = parseFloat($('#VATPercent').val() || 0);

    // 4. Calculate Discount Amount
    let discountAmount = 0;
    if (discountPercent > 0) {
        discountAmount = total * (discountPercent / 100);
        $('#DiscountAmount').val(discountAmount.toFixed(3));
    } else {
        discountAmount = parseFloat($('#DiscountAmount').val() || 0);
    }

    // 5. Calculate VAT Amount
    let vatAmount = 0;
    if (vatPercent > 0) {
        vatAmount = (total - discountAmount) * (vatPercent / 100);
        $('#VATAmount').val(vatAmount.toFixed(3));
        $('#QspanVATAmount').html(numberWithCommas(vatAmount.toFixed(3)));
    } else {
        $('#QspanVATAmount').html(numberWithCommas("0.000"));
    }

    // 6. Final Net Total
    const netTotal = total - discountAmount + vatAmount;

    // Update UI with 3 decimals
    $('#QspanQuotationNetValue').html(numberWithCommas(netTotal.toFixed(3)));
    $('#QuotationValue').val(netTotal.toFixed(3));
}

function calculatequotationvalueold() {
    debugger;
    var idtext = 'QtxtAmount_'
    var total = 0;
    $('[id^=' + idtext + ']').each(function (index, item) {
        if ($('#QDeleted_' + index).val() != 'true') {
            total = parseFloat(total) + parsenumeric($('#QtxtAmount_' + index));
        }
    });
    $('#GrossAmount').val(total);
    $('#QspanGrossAmount').html(numberWithCommas(total));
    //calculatequotationvaluenew();
    if ($('#Margin').val() > 0) {
        total = $('#SellingValue').val();
    }
       
    var _discountpercent = 0;
    var _vatpercent = 5;
    var _vatAmount = 0;
    var _discountamount = 0;
    if ($('#DiscountPercent').val() != '') {
        _discountpercent = $('#DiscountPercent').val();
    }

    if ($('#VATPercent').val() != '') {
        _vatpercent = $('#VATPercent').val();
    }
    else {
        _vatpercent = 0;
    }
    if (parseFloat(_discountpercent) > 0) {
        _discountamount = Math.round(parseFloat(total) * (_discountpercent / 100.0));
        $('#DiscountAmount').val(parseFloat(_discountamount).toFixed(3));
    }
    else {
        if ($('#DiscountAmount').val() == "")
            _discountamount = 0;
        else
            _discountamount  =parseFloat($('#DiscountAmount').val()).toFixed(3);
    }

    if (parseFloat(_vatpercent) > 0) {
        _vatAmount = parseFloat((parseFloat(total)- _discountamount) * _vatpercent/ 100.0).toFixed(3);
        $('#VATAmount').val(_vatAmount);
        $('#QspanVATAmount').html(numberWithCommas(_vatAmount));
    }
    else {
        _vatAmount = 0;
        $('#QspanVATAmount').html(numberWithCommas(0));
    }
   
    var _nettotal = 0;
    _nettotal = parseFloat(total) - parseFloat(_discountamount) + parseFloat(_vatAmount);

    $('#QspanQuotationNetValue').html(numberWithCommas(_nettotal));
    $('#QuotationValue').val(_nettotal);
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
function calculatequotationvaluenew() {
    debugger;

    var total = $('#GrossAmount').val();

    var _marginpercent = $('#MarginPercent').val();
    if (_marginpercent == '') {
        _marginpercent = 0;
        $('#MarginPercent').val(0);
    }
    //I29 / (1 - H30) - I29
   
    var _marginpercent = 1 - parseFloat(_marginpercent) / 100.00;    
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
(function ($) {

    'use strict';
    function initformControl() {
        var suppressOpenOnClear = false;
        $('#QuotationDate').datepicker({
            dateFormat: 'dd-mm-yy',
            changeMonth: true,
            changeYear: true
        }).on('changeDate', function (e) {
            $(this).datepicker('hide');
        });

      
        if ($('#hdnQuotationMode').val() == 'New') {
            $('#QuotationNo').css('color', 'blue');
        }

        $('#QuotationNo').autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: "/Quotation/GetQuotationNo",
                    data: { term: request.term, EnquiryID: $('#EnquiryID').val(), EmployeeID: $('#QEngineerID').val() },
                    dataType: "json",
                    type: "GET",
                    success: function (data) {
                        response($.map(data, function (val, item) {
                            return {
                                label: val.QuotationNo,
                                value: val.QuotationID,
                                Version: val.Version
                            }
                        }))
                    }
                });
            },
            minLength: 1,
            autoFocus: false,
            select: function (event, ui) {
                event.preventDefault();
                $('#QuotationID').val(ui.item.value);
                $('#Version').val(ui.item.Version);

                $('#QuotationNo').val(ui.item.label);

                return false;
            },
            focus: function (event, ui) {
                $('#QuotationID').val(ui.item.value);
                $('#Version').val(ui.item.Version);

                $('#QuotationNo').val(ui.item.label);

                return false;
            }
        });

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
        $("#QtxtEquipmentType").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/Enquiry/GetEquipmentType',
                    datatype: "json",
                    data: {
                        term: request.term, ProductFamilyID: $('#QProductFamilyID').val()
                    },
                    success: function (data) {
                        response($.map(data, function (val, item) {
                            return {
                                label: val.EquipmentTypeName,
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
                $('#QtxtEquipmentType').val(ui.item.label);
                $('#QEquipmentTypeID').val(ui.item.value);

            },
            select: function (e, ui) {
                e.preventDefault();
                $('#QtxtEquipmentType').val(ui.item.label)

                $('#QEquipmentTypeID').val(ui.item.value);

            },

        });

        $("#QtxtBrand").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/Enquiry/GetBrand',
                    datatype: "json",
                    data: {
                        term: request.term
                    },
                    success: function (data) {
                        response($.map(data, function (val, item) {
                            return {
                                label: val.BrandName,
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
                $('#QtxtBrand').val(ui.item.label);


            },
            select: function (e, ui) {
                e.preventDefault();
                $('#QtxtBrand').val(ui.item.label);

            },

        });

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
        $('#QuotationStatusID').blur(function () {
            $('#drpQuotationTo').select2('open');
        })

        $('#drpQuotationTo').blur(function () {
            $('#SubjectText').focus();
        })
        //$("#QuotationPayment").autocomplete({
        //    source: function (request, response) {
        //        $.ajax({
        //            url: '/Enquiry/GetMasterDataList',
        //            datatype: "json",
        //            data: {
        //                'MasterName': 'PaymentTerms', 'term': request.term
        //            },
        //            success: function (response1) {
        //                response($.map(response1.data, function (val, item) {
        //                    return {
        //                        label: val.Text,
        //                        value: val.ID


        //                    }
        //                }))
        //            }
        //        })
        //    },
        //    minLength: 1,
        //    autoFocus: false,
        //    focus: function (e, ui) {
        //        e.preventDefault();
        //        $("#QuotationPayment").val(ui.item.label);

           


        //    },
        //    select: function (e, ui) {
        //        e.preventDefault();
        //        $("#QuotationPayment").val(ui.item.label);

           

        //    },

        //});

        $('#btnaddestimation').click(function () {
            var _estimationID = $('#drpEstimation').val();
            var estimationno = $('#drpEstimation').select2('data')[0]?.text;
            
            $.ajax({
                type: "POST",
                url: '/Quotation/GetEstimationDetail/',
                datatype: "html",
                data: { EstimationID: _estimationID, EnquiryID: $('#EnquiryID').val() },
                success: function (data) {
                    $("#estimationdetailContainer").html(data);
                    $('#divEstimationNo').html('Estimation No.' + estimationno);
                    $('#estimationdetailpopup').modal('show');
                }
            });
        })

        $('#DirectQuotation').change(function () {
            if ($('#DirectQuotation').prop('checked')) {
                $('#flexSwitchCheckChecked').prop('checked', false).trigger('change');
                $('#flexSwitchCheckChecked').attr('disabled', 'disabled');
            }
            else {
                $('#flexSwitchCheckChecked').prop('checked', true).trigger('change');
            }           

        });

        $('#flexSwitchCheckChecked').change(function () {
            if ($('#flexSwitchCheckChecked').prop('checked')) {
                $('#lblflexSwitch').html('Quotation by Estimation');
                $('#divestimationqutation').removeClass('d-xl-none');
                $('#divdirectquotation').addClass('d-xl-none');
                $('#divmargin').addClass('d-xl-none');
            }
            else {
                $('#lblflexSwitch').html('Direct Quotation');
                $('#divestimationqutation').addClass('d-xl-none');
                $('#divdirectquotation').removeClass('d-xl-none');
                $('#divmargin').removeClass('d-xl-none');
            }
            
        });

   
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
                            if (data != null) {
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
        $('.estimation-select').each(function () {
            var $select = $(this);

            
            var defaultOption = new Option($select.attr('boxname'), $select.attr('boxid'), true, true);
            $select.append(defaultOption);
            //// Append it to the select
            $select.append('<option id="" value=""></option>').trigger('change');
            $select.select2({
                placeholder: 'Select a Estimation No.',
                allowClear: false,
                width:"100%",
                minLength: 1,
                ajax: {
                    url: '/Quotation/GetEstimation',
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        // var category = $select.closest('tr').find('.category').val();
                        return {
                            term: params.term,
                            EnquiryID: $('#EnquiryID').val()                            
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


             
        });
       
        
        $('#SelectedQuotationId').change(function () {
            var quotationId = $('#SelectedQuotationId').val();
            if ($('#SelectedQuotationId').val() == '') {
                $('#btnSaveQuotation').html('Add & Save');
                $('#btnprint').addClass('disabled-link');
                $('#btnNewQuotation').addClass('disabled-link');
                $('#btnDelete').addClass('disabled-link');
                clearQuotation();
                showQuotationEntry(0)
                $('#btnprint').attr('href', 'javascript:void(0)');
            }
            else {
                $('#btnSaveQuotation').removeAttr('disabled');
                $('#btnDelete').removeClass('disabled-link');
                $('#btnprint').removeClass('disabled-link');
                $('#btnNewQuotation').removeClass('disabled-link');
                $('#btnSaveQuotation').html('Update');
                showQuotationEntry(quotationId, 0);
                $('#btnprint').attr('href', '/Job/ReportPrint?id=' + $('#JobID').val() + '&option=6&quotationid=' + $('#SelectedQuotationId').val());
            }

        });

        $('#btnNewQuotation').click(function () {
            var QuotationId = 0;
            $.ajax({
                type: "POST",
                url: '/Quotation/ShowQuotationEntry/',
                //datatype: "html",
                data: { Id: QuotationId, EnquiryID: $('#EnquiryID').val(), QuotationID: 0,EmployeeID:$('#QEngineerID').val() },
                success: function (response) {
                    console.log(response.data);
                    var data = response.data;
                    var myDate = new Date(data.QuotationDate.match(/\d+/)[0] * 1);
                    var cmon = myDate.getMonth() + 1;
                    var entrydate = myDate.getDate() + "-" + cmon + "-" + myDate.getFullYear();

                    $('#QuotationID').val(0);
                    $('#QuotationNo').val(data.QuotationNo);
                    $('#QuotationNo').attr('disabled', 'disabled');
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
                    $('#JobID').val(data.JobID);
                    $('#QtxtSubject').val(data.SubjectText);
                    $('#ClientID').val(data.ClientID);
                    $('#ClientDetail').val(data.ClientDetail);
                    $('#EngineerID').val(data.EngineerID).trigger('change');
                    $('#btnSaveQuotation').html('Add & Save');
                    //$("#QuotationEntryContainer").html(data);                       
                    $.ajax({
                        type: "POST",
                        url: '/Quotation/ShowQuotationDetailList/',
                        datatype: "html",
                        data: { EnquiryID: $('EnquiryID').val(),QuotationId: QuotationId },
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
        })
        $('#SelectedQuotationId').trigger('change');

        $('#chkNominalCapacity').change(function () {
            if ($('#chkNominalCapacity').prop('checked') == true) {
                $('#QtxtNominal').removeAttr('disabled');
            } else {
                $('#QtxtNominal').attr('disabled','disabled');
            }
        })

        $('#chkEfficientType').change(function () {
            if ($('#chkEfficientType').prop('checked') == true) {
                $('#QtxtEfficientType').removeAttr('disabled');
            } else {
                $('#QtxtEfficientType').attr('disabled', 'disabled');
            }

        })
        $.ajax({
            url: '/Estimation/GetEstimationCategory',   // 🔹 API or controller action
            type: 'GET',                       // or 'POST' if needed
            dataType: 'json',
            success: function (data) {
                var $ddl = $("#QEstimationCategoryID");
                $ddl.empty(); // clear existing options
                $ddl.append('<option value="">-- Select Item --</option>');

                $.each(data, function (i, item) {
                    $ddl.append($('<option></option>')
                        .val(item.ID)   // backend should send "Value"
                        .text(item.CategoryName)); // backend should send "Text"
                });
            },
            error: function (xhr, status, error) {
                console.error("Error fetching dropdown data:", error);
            }
        });

       
        //$.each(selectedjobid, function (index, item) {
        //    alert(item);
        //    if (item != '') {
        //        var value = data[index].ID;
        //        var label = data[index].Text;


        //        var selected = false;
        //        $.each(selectedjobid, function (val1, item1) {
        //            if (value == item1)
        //                selected = true;
        //        });
        //        if (selected == true) {
        //            var html = '<option selected value="' + value + '">' + label + '</option>';
        //            $('#drpQuotationTo').append(html);
        //        }
        //        else {
        //            var html = '<option value="' + value + '">' + label + '</option>';
        //            $('#drpQuotationTo').append(html);
        //        }
        //    }
        //});
        
    }
    function init() {
        initformControl();
    }
    $(document).ready(function () {
        init();
       
        //document.addEventListener("click", function (e) {
        //    var dropdown = document.getElementById("drptxtTerms");
        //    if (!dropdown.contains(e.target)) {
        //          $("#drptxtTerms").css('display', 'none');

        //    }
        //});
        //// Optional: hide when clicking outside
        //document.addEventListener("click", function (e) {
        //    var dropdown = document.querySelector(".dropdown");
        //    console.log(drpdown);
        //    if (!dropdown.contains(e.target)) {
        //        document.getElementById("drptxtTerms").style.display = "none";
        //    }
        //});
        //for entitty type binding

        //$('#drpQuotationTo').select2({
        //    placeholder: 'Select a Client',
        //    allowClear: true,
        //    minLength: 1,
        //    ajax: {
        //        url: '/Quotation/GetClientName',
        //        dataType: 'json',
        //        delay: 250,
        //        data: function (params) {
        //            // var category = $select.closest('tr').find('.category').val();
        //            return {
        //                term: params.term,
        //                EnquiryID: $('#EnquiryID').val()
        //            };
        //        },
        //        processResults: function (data) {
        //            return {
        //                results: data.map(function (item) {
        //                    return { id: item.ClientID, text: item.ClientName };
        //                })
        //            };
        //        },
        //        cache: true
        //    }
        //});
        $.ajax({
            type: "Post",
            url: '/Quotation/GetClientName',
            data: {
                'term': '', 'EnquiryID': $('#EnquiryID').val()
            },
            success: function (response) {
                debugger;
                                    
                    var data = response;
                    var selectedjobid = [];
                    if ($('#QuotationTo').val() != '') {
                        selectedjobid = $('#QuotationTo').val().split(',');
                    }
                    $.each(data, function (index, item) {
                        var value = data[index].ClientID;
                        var label = data[index].ClientName;


                        var selected = false;
                        $.each(selectedjobid, function (val1, item1) {
                            if (value == item1)
                                selected = true;
                        });
                        if (selected == true) {
                            var html = '<option selected value="' + value + '">' + label + '</option>';
                            $('#drpQuotationTo').append(html);
                        }
                        else {
                            var html = '<option value="' + value + '">' + label + '</option>';
                            $('#drpQuotationTo').append(html);
                        }
                    })
                
                $('#drpQuotationTo').select2({
                    width: '100%'                
                });


                
            }
        });

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

                            SaveQuotation();

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
        $(".custom-validation").parsley();
       
        $('#btnviewestimation').click(function () {
            var _estimationID = $('#drpEstimation').val();            
            window.open(
                "/Estimation/Create?id=" + _estimationID,
                '_blank' // <- This is what makes it open in a new window.
            );
        })


        //window.addEventListener('click', function (event) {
        //    var drpTerm = document.getElementById('QtxtTerms');
        //    console.log(event.target);
        //    if (!drpTerm.contains(event.target)) {
               
        //        if ($('#drptxtTerms').css('display') == 'block') {
        //            toggleDropdown();
        //        }
        //    }
        //    else {
        //        //toggleDropdown();
        //    }
        //});
        //tinymce.init({
        //    selector: "textarea#elm2",
        //    height: 300,
        //    plugins: [
        //        "advlist autolink link image lists charmap print preview hr anchor pagebreak spellchecker",
        //        "searchreplace wordcount visualblocks visualchars code fullscreen insertdatetime media nonbreaking",
        //        "save table contextmenu directionality emoticons template paste textcolor",
        //    ],
        //    toolbar: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | l      ink image | print preview media fullpage | forecolor backcolor emoticons",
        //    style_formats: [
        //        { title: "Bold text", inline: "b" },
        //        { title: "Red text", inline: "span", styles: { color: "#ff0000" } },
        //        { title: "Red header", block: "h1", styles: { color: "#ff0000" } },
        //        { title: "Example 1", inline: "span", classes: "example1" },
        //        { title: "Example 2", inline: "span", classes: "example2" },
        //        { title: "Table styles" },
        //        { title: "Table row 1", selector: "tr", classes: "tablerow1" },
        //    ],
        //});

        //0 < $("#elm1").length &&
        //    tinymce.init({
        //        selector: "textarea#elm1",
        //        height: 300,
        //        plugins: [
        //            "advlist autolink link image lists charmap print preview hr anchor pagebreak spellchecker",
        //            "searchreplace wordcount visualblocks visualchars code fullscreen insertdatetime media nonbreaking",
        //            "save table contextmenu directionality emoticons template paste textcolor",
        //        ],
        //        toolbar: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | l      ink image | print preview media fullpage | forecolor backcolor emoticons",
        //        style_formats: [
        //            { title: "Bold text", inline: "b" },
        //            { title: "Red text", inline: "span", styles: { color: "#ff0000" } },
        //            { title: "Red header", block: "h1", styles: { color: "#ff0000" } },
        //            { title: "Example 1", inline: "span", classes: "example1" },
        //            { title: "Example 2", inline: "span", classes: "example2" },
        //            { title: "Table styles" },
        //            { title: "Table row 1", selector: "tr", classes: "tablerow1" },
        //        ],
        //    });


     
           
        //0 < $("#elm3").length &&
        //    tinymce.init({
        //        selector: "textarea#elm3",
        //        height: 300,
        //        plugins: [
        //            "advlist autolink link image lists charmap print preview hr anchor pagebreak spellchecker",
        //            "searchreplace wordcount visualblocks visualchars code fullscreen insertdatetime media nonbreaking",
        //            "save table contextmenu directionality emoticons template paste textcolor",
        //        ],
        //        toolbar: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | l      ink image | print preview media fullpage | forecolor backcolor emoticons",
        //        style_formats: [
        //            { title: "Bold text", inline: "b" },
        //            { title: "Red text", inline: "span", styles: { color: "#ff0000" } },
        //            { title: "Red header", block: "h1", styles: { color: "#ff0000" } },
        //            { title: "Example 1", inline: "span", classes: "example1" },
        //            { title: "Example 2", inline: "span", classes: "example2" },
        //            { title: "Table styles" },
        //            { title: "Table row 1", selector: "tr", classes: "tablerow1" },
        //        ],
        //    });

        

        if ($('#QuotationID').val() > 0) {
            setTimeout(function () {
                $('#QuotationPayment').val($('#QuotationPayment').attr('svalue'));
                //var _scopeofwork = $('#ScopeofWork').val();
                //var _warranty = $('#Warranty').val();
                //var _exclusions = $('#Exclusions').val();
                //tinymce.get('elm1').setContent(decodeURIComponent(_scopeofwork));
                //tinymce.get('elm2').setContent(decodeURIComponent(_warranty));
                //tinymce.get('elm3').setContent(decodeURIComponent(_exclusions));
            }, 200)

            if ($('#DirectQuotation').prop('checked')) {
                $('#flexSwitchCheckChecked').prop('checked', false).trigger('change');
                $('#flexSwitchCheckChecked').attr('disabled', 'disabled');
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
            else {
                $('#flexSwitchCheckChecked').prop('checked', true).trigger('change');
            }
            $('#ProjectRef').focus();
        }
        else {
            $('#ProjectRef').focus();
        }
    });
