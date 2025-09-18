function validatePurchaseOrderForm() {
    var isValid = true;

    // Helper to show/hide error
    function setError(selector, message) {
        $(selector).addClass('input-validation-error');
        $(selector).siblings('.field-validation-error').remove();
        $(selector).after('<span class="field-validation-error" style="color:red;font-size:12px;"><i class="mdi mdi-alert-circle-outline"></i> ' + message + '</span>');
    }
    function clearError(selector) {
        $(selector).removeClass('input-validation-error');
        $(selector).siblings('.field-validation-error').remove();
    }

    // Validate Purchase Order Date
    if (!$('#PurchaseOrderDate').val()) {
        setError('#PurchaseOrderDate', 'Required');
        isValid = false;
    } else {
        clearError('#PurchaseOrderDate');
    }

    // Validate Supplier
    if (!$('#SupplierID').val() || $('#SupplierID').val() == '') {
        setError('#SupplierID', 'Required');
        isValid = false;
    } else {
        clearError('#SupplierID');
    }

    // Validate Payment Terms
    if (!$('#PaymentTermID').val() || $('#PaymentTermID').val() == '') {
        setError('#PaymentTermID', 'Required');
        isValid = false;
    } else {
        clearError('#PaymentTermID');
    }

    // Validate Delivery Terms
    if (!$('#DeliveryTermID').val() || $('#DeliveryTermID').val() == '') {
        setError('#DeliveryTermID', 'Required');
        isValid = false;
    } else {
        clearError('#DeliveryTermID');
    }

    // Validate INCOTerms
    if (!$('#IncoTermID').val() || $('#IncoTermID').val() == '') {
        setError('#IncoTermID', 'Required');
        isValid = false;
    } else {
        clearError('#IncoTermID');
    }

    // Validate Bank
    if (!$('#po_Bank').val() || $('#po_Bank').val() == '') {
        setError('#po_Bank', 'Required');
        isValid = false;
    } else {
        clearError('#po_Bank');
    }
    var emptyrow = $('#PurchaseOrderEquipmentTables > tbody > tr').html();
    if (emptyrow != undefined) {
        if (emptyrow.indexOf('No data available in table') >= 0) {
            $('#PurchaseOrderEquipmentTables > tbody').html('');
        }
        $('#PurchaseOrderEquipmentTables').addClass('input-validation-error');
        
        
    }
    var itemcount = $('#PurchaseOrderEquipmentTables > tbody > tr').length;


    if (itemcount == 0) {
        $('#spanerr').html('Enter Quotation Item Detail!');
        if ($('#PurchaseOrderEquipmentTables').next('.field-validation-error').length === 0) {
            $('#PurchaseOrderEquipmentTables').after('<span class="field-validation-error" style="color:red;font-size:12px;"><i class="mdi mdi-alert-circle-outline"></i> At least one equipment item required</span>');
        }
        isValid = false;
        return false;
    } else {
        $('#PurchaseOrderEquipmentTables').removeClass('input-validation-error');
        $('#PurchaseOrderEquipmentTables').next('.field-validation-error').remove();
    }
    //// Validate Equipment items
    //if (dataArray.length === 0) {
    //    $('#PurchaseOrderEquipmentTables').addClass('input-validation-error');
    //    if ($('#PurchaseOrderEquipmentTables').next('.field-validation-error').length === 0) {
    //        $('#PurchaseOrderEquipmentTables').after('<span class="field-validation-error" style="color:red;font-size:12px;"><i class="mdi mdi-alert-circle-outline"></i> At least one equipment item required</span>');
    //    }
    //    isValid = false;
    //} else {
    //    $('#PurchaseOrderEquipmentTables').removeClass('input-validation-error');
    //    $('#PurchaseOrderEquipmentTables').next('.field-validation-error').remove();
    //}

    // Validate Regrigerant
    if (!$('#drpRefrigerantID').val() || $('#drpRefrigerantID').val() == '') {
        setError('#drpRefrigerantID', 'Required');
        isValid = false;
    } else {
        clearError('#drpRefrigerantID');
    }

    // Validate Compressor Warranty
    if (!$('#CompressorWarrantyID').val() || $('#CompressorWarrantyID').val() == '') {
        setError('#CompressorWarrantyID', 'Required');
        isValid = false;
    } else {
        clearError('#CompressorWarrantyID');
    }

    if (isValid) {
        savePurchaseOrder();
    }
}
function savePurchaseOrder() {
    debugger;
    var paymentTermsText = $('#PaymentTermID').select2('data')[0]?.text || '';
    var deliveryTermsText = $('#DeliveryTermID').select2('data')[0]?.text || '';
    var refrigerantitems = $('#drpRefrigerantID').val();
    var refrigerants = '';
    if (refrigerantitems != null) {

        $.each(refrigerantitems, function (index, item) {
            if (item != '') {
                refrigerants = refrigerants + ',' + item;
            }
            $('#RefrigerantID').val(refrigerants);
        });
    }
    let notes = [];
    $('.note-text').each(function () {
        let noteText = $(this).val().trim();
        if (noteText !== "") {
            notes.push({ Notes: noteText });
        }
    });

    var purchaseOrder = {
        PurchaseOrderID: $('#po_PurchaseOrderID').val(),
        PurchaseOrderNo: $("#po_PurchaseOrderNo").val(),
        PurchaseOrderDate: $("#PurchaseOrderDate").val(),
        SupplierID: $("#SupplierID").val(),
        SONoRef: $("#po_SONoRef").val(),
        PaymentTerms: paymentTermsText,
        DeliveryTerms: deliveryTermsText,
        INCOTerms: $("#IncoTermID").val(),
        Bank: $("#po_Bank").val(),
        POValue: $("#txtPOValue").val(),
        VATPercent: $("#QtxtVATPercent").val(),
        VATAmount: parsenumeric($("#QtxtVATAmount")),
        TotalAmount: parsenumeric($("#txtTotalAmount")),
        Remarks: $("#po_Remarks").val(),
        Refrigerant: $('#RefrigerantID').val(),
        CompressorWarranty: $('#CompressorWarrantyID').val(),
        FreightCharges: parsenumeric($("#QtxtFreightCharges")),
        OriginCharges: parsenumeric($("#QtxtOriginCharges")),
        FinanceCharges: $("#QtxtFinanceCharges").val(),
        FinancePercent: $("#QtxtFinanceChargePercent").val(),
        UnitWarrantyID: $('#UnitWarrantyID').val(),
        CurrencyID: $('#po_CurrencyID').val(),
        RevisionRemarks: $('#po_RevisionRemarks').val(),
        PreviousValue: $('#po_PreviousValue').val(),
        OriginCountryID: $('#po_OriginCountryID').val(),
        PortID: $('#po_PortID').val(),
        Revision: $('#po_Revision').val(),        
    };

    var otherorderDetails = [{
        OrderDate: $("#QtxtOrderDate").val(),
        DrawingApprovalDate: $("#QtxtDrawingApprovalDate").val(),
        ExFactoryDate: $("#QtxtExFactoryDate").val(),
        ETD: $("#QtxtETD").val(),
        ETA: $("#QtxtETA").val(),
        Remarks: $("#DetailsRemarks").val()
    }];
    var itemcount = $('#PurchaseOrderEquipmentTables > tbody > tr').length;
    var quotationdetails = [];
    for (i = 0; i < itemcount; i++) {
        var quotationdetail = {
            PurchaseOrderDetailID: $('#QPurchaseOrderDetailID_' + i).val(),
            PurchaseOrderID: $('#PurchaseOrderID').val(),
            QuotationID: $('#QQuotationID_' + i).val(),
            EquipmentID: $('#QEquipmentID_' + i).val(),
            MRequestID: $('#QMRequestID_' + i).val(),
            EquipmentTypeID: $('#QEquipmentTypeID_' + i).val(),
            EstimationID: $('#QEstimationID_' + i).val(),
            Description: $('#QtxtDescription_' + i).val(),
            Model: $('#QtxtModel_' + i).val(),
            ItemUnitID: $('#QUnitID_' + i).val(),
            Quantity: $('#QtxtQty_' + i).val(),
            Rate: parsenumeric($('#QtxtUnitRate_' + i)),
            Amount: parsenumeric($('#QtxtAmount_' + i)),
            Deleted: $('#QDeleted_' + i).val(),
            ProjectNo: $('#QProjectNo_' + i).val(),
            JobHandOverID: $('#QJobHandOverID_' + i).val()
        }

        quotationdetails.push(quotationdetail);


        if (itemcount == (i + 1)) {
            $.ajax({
                url: '/PurchaseOrder/SaveEquipmentRow',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({
                    po: purchaseOrder,
                    Details: quotationdetails,
                    othercharges: null,
                    comment: null,
                    Notes:notes,
                    orderdetails: otherorderDetails
                }),
                success: function (response) {
                    if (response.success) {
                        Swal.fire({
                            title: "Success!",
                            text: response.message || "Data saved successfully.",
                            icon: "success",
                            confirmButtonColor: "#34c38f"
                        }).then(() => {
                            window.location.href = "/PurchaseOrder/Create/" + response.PurchaseOrderID;
                        });
                    } else {
                        Swal.fire({
                            title: "Error!",
                            text: response.message || "There was an error saving the data.",
                            icon: "error",
                            confirmButtonColor: "#f46a6a"
                        });
                    }

                    clearall();
                    console.log(response);
                },
                error: function (xhr, status, error) {
                    alert('Error saving data');
                    console.error(xhr.responseText);
                }
            });
        }
    }

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
    calculatePoValue();
}
function calculatePoValue() {
    let grossAmountTotal = 0;
    var _vatAmount = 0;
    var _vatpercent = parseFloat($('#QtxtVATPercent').val()) || 0;

    

    //// Sum all item.Value from dataArray
    //$.each(dataArray, function (index, item) {
    //    if (item.Value && !isNaN(item.Value)) {
    //        grossAmountTotal += parseFloat(item.Value);
    //    }
    //});
    var total = 0;
    var _jobids = "";
    var idtext = 'QtxtAmount_';
    $('[id^=' + idtext + ']').each(function (index, item) {
        if ($('#QDeleted_' + index).val() != 'true') {
            total = parseFloat(total) + parsenumeric($('#QtxtAmount_' + index));
            _jobids = _jobids + "," + $('#QJobHandOverID_' + index).val();
        }
    });
    $('#txtTotalAmount').val(total);
    // Update the gross amount display
    $('#spanPurchaseGrossAmount').html(numberWithCommas(total));

    $('#spanVATAmount').html(numberWithCommas(_vatAmount));
    
    

    // Add Freight, Origin, and Finance Charges
    let freight = parseFloat($('#QtxtFreightCharges').val()) || 0;
    let origin = parseFloat($('#QtxtOriginCharges').val()) || 0;
    
    grossAmountTotal = parseFloat(total) + freight + origin;

    $('#QtxtSubTotal').val((parseFloat(grossAmountTotal)));

    
    $('#spanSubTotal1').html(numberWithCommas(grossAmountTotal));

    var _Paymentstermsmonth = $('#PaymentTermID').val() / 30;

    var financecharges = (parseFloat(grossAmountTotal)) * ($('#QtxtFinanceChargePercent').val() / 100) * _Paymentstermsmonth;

    $('#QtxtFinanceCharges').val(parseFloat(financecharges).toFixed(3));

    let finance = parseFloat($('#QtxtFinanceCharges').val()) || 0;
    let subtotal2 = parseFloat(grossAmountTotal) + finance;
    
    $('#spanSubTotal2').html(numberWithCommas(subtotal2));
  
    if (parseFloat(_vatpercent) > 0) {
        _vatAmount = Math.round((parseFloat(subtotal2)) * (_vatpercent / 100.0));
    }
    
    $('#QtxtVATAmount').val(_vatAmount);    
    // Update the new footer spans
    //$('#spanFreightCharge').html(numberWithCommas(freight));
    //$('#spanOriginCharge').html(numberWithCommas(origin));
    //$('#spanFinanceCharge').html(numberWithCommas(finance));

    var _nettotal = 0;
    _nettotal = (parseFloat(subtotal2)) + parseFloat(_vatAmount);

    $('#spanPOValue').html(numberWithCommas(_nettotal));    
    $('#txtPOValue').val(_nettotal);

   
}
//add direct po entry not used
function AddDetail(obj) {
    debugger;

    var itemcount = $('#PurchaseOrderEquipmentTables > tbody > tr').length;

    $(obj).attr('disabled', 'disabled');


    var _Equipmentname = '';
    var _EquipmentTypeId = 0;
        
    _Equipmentname = $('#eEquipmentType').select2('data')[0]?.text;
    _EquipmentTypeId = $('#eEquipmentType').val();

    var quotationdetail = {
        JobHandOverID: 0,
        EstimationID: 0,
        EquipmentID: 0,
        ProjectNo: "N/A",
        EquipmentTypeID: _EquipmentTypeId,
        EstimationNo: "",
        QuotationID: 0,
        UnitName: $('#QUnitID option:selected').text(),
        ItemUnitID: $('#QUnitID_' + i).val(),
        Description: _Equipmentname,
        Model: $('#QtxtModel').val(),
        Quantity: $('#QtxtQty').val(),
        Rate: parsenumeric($('#QtxtRate')),
        Amount: parsenumeric($('#QtxtValue'))

    }

    var quotationdetails = [];
    var quotationdetail = {
        EquipmentID: $('#QEquipmentID_' + k).val(),
        EstimationID: $('#QEstimationID_' + k).val(),
        JobHandOverID: $('#QJobHandOverID_' + k).val(),
        EquipmentTypeID: $('#QEquipmentTypeID_' + k).val(),
        QuotationID: $('#QQuotationID_' + k).val(),
        EstimationNo: $('#QEstimationNo_' + k).val(),
        UnitName: $('#QUnitName_' + k).val(),
        ItemUnitID: $('#QUnitID_' + k).val(),
        Description: $('#QtxtDescription_' + k).val(),
        Model: $('#QtxtModel_' + k).val(),
        Quantity: $('#QtxtQty_' + k).val(),
        Rate: parsenumeric($('#QtxtUnitRate_' + k)),
        Amount: parsenumeric($('#QtxtAmount_' + k)),
        Deleted: $('#QDeleted_' + k).val()
    }

    quotationdetails.push(quotationdetail);


    $.ajax({
        type: "POST",
        url: '/PurchaseOrder/AddItem/',
        datatype: "html",
        data: { invoice: equipmententry,  Details: JSON.stringify(quotationdetails) },
        success: function (data) {
            $("#PurchaseOrderContainer").html(data);
            $(obj).removeAttr('disabled');
            $('#QEquipmentCategory').focus()
            calculatePoValue();
            //clearDetail();
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
                
        var _estimationNo = $('#estimationdetailpopup').attr('EstimationNo');
        var quotationdetail = {
            JobHandOverID: $('#eJobHandOverID_' + i).val(),
            EstimationID: $('#eEstimationID_' + i).val(),
            EquipmentID: $('#eEquipmentID_' + i).val(),
            ProjectNo: $('#eProjectNo_' + i).val(),
            EquipmentTypeID: $('#eEquipmentTypeID_' + i).val(),
            EstimationNo: $('#eEstimationNo_' + i).val(),
            QuotationID: $('#eQuotationID_' + i).val(),                        
            UnitName: $('#eUnitName_' + i).val(),
            ItemUnitID: $('#eUnitID_' + i).val(),            
            Description: $('#etxtDescription_' + i).val(),
            Model: $('#etxtModel_' + i).val(),
            Quantity: $('#etxtQty_' + i).val(),
            Rate: parsenumeric($('#etxtRate_' + i)),
            Amount: parsenumeric($('#etxtLValue_' + i)),
            Checked: $('#chkEstimationItem_' + i).prop('checked')
            
        }

        quotationdetails.push(quotationdetail);

    }


    $.ajax({
        type: "POST",
        url: '/PurchaseOrder/AddEstimationItem/',
        datatype: "html",
        data: { Details: JSON.stringify(quotationdetails) },
        success: function (response) {
            debugger;
            var quotationdetails = [];
            var itemcount1 = $('#PurchaseOrderEquipmentTables > tbody > tr').length;
            if (itemcount1 == 0) {
                $.ajax({
                    type: "POST",
                    url: '/PurchaseOrder/AddItem1/',
                    datatype: "html",
                    data: { Details: JSON.stringify(response.data), Details1: JSON.stringify(quotationdetails), JobID: 0 },
                    success: function (data1) {
                        $("#PurchaseOrderContainer").html(data1);
                        $('#btnAddEstimationDetail').removeAttr('disabled');
                        calculatePoValue();
                        //calculatequotationvalue();
                        //clearDetail();
                        $('#estimationdetailpopup').modal('hide');
                    }
                });
            } else {
                for (k = 0; k < itemcount1; k++) {

                    

                    var quotationdetail = {
                        EquipmentID: $('#QEquipmentID_' + k).val(),
                        EstimationID: $('#QEstimationID_' + k).val(),
                        JobHandOverID: $('#QJobHandOverID_' + k).val(),
                        EquipmentTypeID: $('#QEquipmentTypeID_' + k).val(),
                        QuotationID: $('#QQuotationID_' + k).val(),
                        EstimationNo: $('#QEstimationNo_' + k).val(),
                        UnitName: $('#QUnitName_' + k).val(),
                        ItemUnitID: $('#QUnitID_' + k).val(),                        
                        Description: $('#QtxtDescription_' + k).val(),
                        Model: $('#QtxtModel_' + k).val(),
                        Quantity: $('#QtxtQty_' + k).val(),
                        Rate: parsenumeric($('#QtxtUnitRate_' + k)),
                        Amount: parsenumeric($('#QtxtAmount_' + k)),
                        Deleted: $('#QDeleted_' + k).val()                        
                    }

                    quotationdetails.push(quotationdetail);

                    if ((k + 1) == itemcount1) {
                        $.ajax({
                            type: "POST",
                            url: '/PurchaseOrder/AddItem1/',
                            datatype: "html",
                            data: { Details: JSON.stringify(response.data), Details1: JSON.stringify(quotationdetails), JobID: 0 },
                            success: function (data1) {
                                $("#PurchaseOrderContainer").html(data1);
                                $('#btnAddEstimationDetail').removeAttr('disabled');
                                $('#QEquipmentCategory').focus();
                                calculatePoValue();
                                //calculatequotationvalue();
                                //clearDetail();
                                $('#estimationdetailpopup').modal('hide');
                            }
                        });

                    }

                }

            }


        }
    });

}
function checkdeletedentry() {
    var idtext = 'poeq_row_'
    $('[id^=' + idtext + ']').each(function (index, item) {
        if ($('#QDeleted_' + index).val() == 'True') {
            $('#poeq_row_' + index).addClass('hide');
        }
        else {
            $('#poeq_row_' + index).removeClass('hide');
        }
    });

}
function DeleteQuotationDetailEntry(index) {
    $('#QDeleted_' + index).val(true);
    $('#quodtr_' + index).addClass('hide');
    calculatePoValue();
}

function SaveApproverEntry() {
    var obj = {

        EmployeeID: $('#drpApproveEmployeeID').val(),
        Type: $('#drpApproveType').val(),
        ValidateText: $('#txtValidate').val(),
        PurchaseOrderID:$('#Id').val()
    }
    $.ajax({
        type: "POST",
        url: '/PurchaseOrder/SaveApprover/',
        datatype: "jsob",
        data: obj,
        success: function (response) {
            alert(response.Message);
        }
    });
}
(function ($) {



    'use strict';
    function initformControl() {
       

        $('.docs-date').datepicker({
            dateFormat: 'dd-mm-yy',   // jQuery UI date format
            showOnFocus: false,       // prevent popup on focus
            autoclose: true,
            todayHighlight: true,
            container: $('.docs-datepicker-container') // if needed
        }).on('blur change', function () {
            let val = $(this).val().trim();

            // Match dd-mm only (e.g. 15-07)
            if (/^\d{2}-\d{2}$/.test(val)) {
                let parts = val.split("-");
                let currentYear = new Date().getFullYear();
                let fullDate = `${parts[0]}-${parts[1]}-${currentYear}`;
                $(this).datepicker('setDate', fullDate);  // sets the corrected date
            }

            // Match dd-mm-yyyy → do nothing (already full)
            else if (/^\d{2}-\d{2}-\d{4}$/.test(val)) {
                // optional: validate
            }

            // Invalid format → clear or highlight
            else if (val !== "") {
                //alert("Invalid date format! Please enter dd-mm or dd-mm-yyyy");
                $(this).val(""); // clear invalid entry
            }
        });

        // ALT + ↓ opens datepicker manually
        $('.docs-date').on('keydown', function (e) {
            if (e.altKey && e.key === "ArrowDown") {
                e.preventDefault();
                $(this).datepicker('show');
            }
        });

        // Common function to open the datepicker
        function openDatepicker(input) {
            $(input).data("manual-open", true).datepicker("show");
            $(input).data("manual-open", false);
        }
        // Prevent auto-popup on focus
        $(document).on('focus', '.docs-date', function (e) {
            $(this).datepicker('hide');  // Hide if it tries to open
        });
        //// Click on calendar icon
        $(document).on('click', '.docs-datepicker-trigger', function () {
            let input = $(this).closest('.docs-datepicker').find('.docs-date');
            openDatepicker(input);
        });

        //// ALT + ↓ key inside the input
        $(document).on('keydown', '.docs-date', function (e) {
            if (e.altKey && e.key === "ArrowDown") {
                e.preventDefault(); // Prevent default scroll
                openDatepicker(this);
            } else {
                return true;
            }
        });
        var _decimal = "2";
        var accounturl = '/Accounts/GetHeadsForCash';
        $('#SupplierID').select2({
            width: 'resolve', // respects width set in the HTML
            placeholder: 'Select a Supplier',
            allowClear: false,
            minLength: 1,
            ajax: {
                url: '/Supplier/GetSupplierName',
                dataType: 'json',
                delay: 250,
                data: function (params) {                
                    return {
                        term: params.term                        
                    };
                },
                processResults: function (data) {
                    return {
                        results: data.map(function (item) {
                            return { id: item.SupplierID, text: item.SupplierName };
                        })
                    };
                },
                cache: true
            }
        });
      


        $('#PaymentTermID').select2({
            width: 'resolve', // respects width set in the HTML
            placeholder: 'Select a Payment Terms',
            allowClear: true,
            ajax: {
                url: '/PurchaseOrder/GetPaymentTerm',
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    return {
                        term: params.term
                    };
                },
                processResults: function (data) {
                    return {
                        results: data.map(function (item) {
                            return {
                                id: item.ID,
                                text: item.TermsText
                            };
                        })
                    };
                },
                cache: true
            }
        });

      

        $('#DeliveryTermID').select2({
            width: 'resolve', // respects width set in the HTML
            placeholder: 'Select a Delivery Terms',
            allowClear: true,
            ajax: {
                url: '/PurchaseOrder/GetDeliveryTerm',
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    return {
                        term: params.term
                    };
                },
                processResults: function (data) {
                    return {
                        results: data.map(function (item) {
                            return {
                                id: item.ID,
                                text: item.TermsText
                            };
                        })
                    };
                },
                cache: true
            }
        });
 


        $('#IncoTermID').select2({
            width: 'resolve', // respects width set in the HTML
            placeholder: 'Select a Inco Term',
            allowClear: true,
            ajax: {
                url: '/PurchaseOrder/GetIncoTerm',
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    return {
                        term: params.term
                    };
                },
                processResults: function (data) {
                    return {
                        results: data.map(function (item) {
                            return {
                                id: item.ID,
                                text: item.TermsText
                            };
                        })
                    };
                },
                cache: true
            }
        });



        //$('#BankID').select2({
        //    width: 'resolve', // respects width set in the HTML
        //    placeholder: 'Select a Bank Name',
        //    allowClear: true,
        //    ajax: {
        //        url: '/PurchaseOrder/GetBankName',
        //        dataType: 'json',
        //        delay: 250,
        //        data: function (params) {
        //            return {
        //                term: params.term
        //            };
        //        },
        //        processResults: function (data) {
        //            return {
        //                results: data.map(function (item) {
        //                    return {
        //                        id: item.ID,
        //                        text: item.BankName
        //                    };
        //                })
        //            };
        //        },
        //        cache: true
        //    }
        //});


        //$('#drpRefrigerantID').select2({
        //    width: 'resolve', // respects width set in the HTML
        //    placeholder: 'Select a Regrigerant',
        //    allowClear: true,
        //    ajax: {
        //        url: '/PurchaseOrder/GetRegrigerant',
        //        dataType: 'json',
        //        delay: 250,
        //        data: function (params) {
        //            return {
        //                term: params.term
        //            };
        //        },
        //        processResults: function (data) {
        //            return {
        //                results: data.map(function (item) {
        //                    return {
        //                        id: item.ID,
        //                        text: item.Regrigerant  // ✅ Correct property name
        //                    };
        //                })
        //            };
        //        },
        //        cache: true
        //    }
        //});

        //for refreigerant binding
        $.ajax({
            type: "GET",
            url: '/PurchaseOrder/GetRegrigerant',
            data: {
                 'term': ""
            },
            success: function (data) {
                debugger;
                    var selectedjobid = [];
                if ($('#RefrigerantID').val() != '') {
                         selectedjobid = $('#RefrigerantID').val().split(',');
                    }
                    $.each(data, function (index, item) {
                        var value = data[index].ID;
                        var label = data[index].Regrigerant;


                        var selected = false;
                        $.each(selectedjobid, function (val1, item1) {
                            if (value == item1)
                                selected = true;
                        });
                        if (selected == true) {
                            var html = '<option selected value="' + value + '">' + label + '</option>';
                            $('#drpRefrigerantID').append(html);
                        }
                        else {
                            var html = '<option value="' + value + '">' + label + '</option>';
                            $('#drpRefrigerantID').append(html);
                        }
                    })
                $('#drpRefrigerantID').select2();
                 
            }
        });


        $('#CompressorWarrantyID').select2({
            width: 'resolve', // respects width set in the HTML
            placeholder: 'Select a Compressor Warranty',
            allowClear: true,
            ajax: {
                url: '/PurchaseOrder/GetCompressorWarranty',
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    return {
                        term: params.term
                    };
                },
                processResults: function (data) {
                    return {
                        results: data.map(function (item) {
                            return {
                                id: item.ID,
                                text: item.CompressorWarranty // ✅ Corrected
                            };
                        })
                    };
                },
                cache: true
            }
        });


        $('#UnitWarrantyID').select2({
            width: 'resolve', // respects width set in the HTML
            placeholder: 'Select a Unit Warranty',
            allowClear: true,
            ajax: {
                url: '/PurchaseOrder/GetUnitWarranty',
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    return {
                        term: params.term
                    };
                },
                processResults: function (data) {
                    return {
                        results: data.map(function (item) {
                            return {
                                id: item.ID,
                                text: item.CompressorWarranty // ✅ Corrected
                            };
                        })
                    };
                },
                cache: true
            }
        });

        


        $('#EquipmentName').autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: "/PurchaseOrder/GetEquipmentType",
                    data: { term: request.term },
                    dataType: "json",
                    type: "GET",
                    success: function (data) {
                        response($.map(data, function (val, i) {
                            return {
                                label: val.EquipmentName,
                                value: val.EquipmentName,
                                id: val.ID
                            };
                        }));
                    }
                });
            },
            minLength: 1,
            autoFocus: true,
            select: function (event, ui) {
                $('#EquipmentName').val(ui.item.label);   // show name in textbox
                $('#EquipmentID').val(ui.item.id);        // store ID in hidden field
                return false;
            },
            focus: function (event, ui) {
                $('#EquipmentName').val(ui.item.label);
                return false;
            }
        });


        var suppressOpenOnClear = false;
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
                width: 'resolve', // respects width set in the HTML
                placeholder: 'Select a Equipment Type',
                allowClear: false,
                minLength: 1,
                ajax: {
                    url: "/PurchaseOrder/GetEquipmentType",
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        // var category = $select.closest('tr').find('.category').val();
                        return {
                            term: params.term 
                        };
                    },
                    processResults: function (data) {
                        return {
                            results: data.map(function (item) {
                                return { id: item.ID, text: item.EquipmentName };
                            })
                        };
                    },
                    cache: true
                }
            });
 

        });
        
        $('#PortID').select2({
            placeholder: 'Select a Port',
            allowClear: false,
            minLength: 1,
            ajax: {
                url: '/Port/GetPortName',
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    return {
                        term: params.term
                    };
                },
                processResults: function (data) {
                    return {
                        results: data.map(function (item) {
                            return { id: item.PortID, text: item.PortName };
                        })
                    };
                },
                cache: true
            }
        });
        //GetProjectNo
        $('#ProjectNo').select2({
            width: 'resolve', // respects width set in the HTML
                placeholder: 'Select a Project',
                allowClear: false,
                minLength: 1,
                ajax: {
                    url: '/Enquiry/GetProjectNo',
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        return {
                            term: params.term
                        };
                    },
                    processResults: function (data) {
                        return {
                            results: data.map(function (item) {
                                return { id: item.JobHandOverID, text: item.ProjectNumber };
                            })
                        };
                    },
                    cache: true
                }
        });

      
        $('#CurrencyID').change(function () {
            $.ajax({
                url: "/Currency/GetExchangeRate",
                data: { CurrencyID: $('#CurrencyID').val() },
                dataType: "json",
                type: "GET",
                success: function (data) {
                    
                    $('#ExchangeRate').val(data);
                    
                    $('#lblExRate').html(data);
                }
            });
        })


        $('#PaymentTermID').change(function () {
            calculatePoValue();
        })

        $('#ChkVATPercent').change(function () {
            if ($('#ChkVATPercent').prop('checked')) {
                $('#QtxtVATPercent').val(5);
                calculatePoValue();
            }
            else {
                $('#QtxtVATPercent').val(0);
                calculatePoValue();
            }            
        })
        //$('#po_Revision').blur(function () {
        //    $('#BankID').select2('open');
        //})
        //$('#BankID').blur(function () {
        //    $('#po_PortID').focus();
        //})

        //$('#po_PortID').blur(function () {
        //    $('#po_OriginCountryID').focus();
        //})

        //$('#po_SONoRef').blur(function () {
        //    $('#UnitWarrantyID').select2('open');
        //})
        //$('#UnitWarrantyID').blur(function () {
        //    $('#CompressorWarranty').select2('open');
        //})

        //$('#CompressorWarranty').blur(function () {
        //    $('#PaymentTermID').select2('open');
        //})
        

        if ($("#ProductTypeID").val() > 0) {
            $('#btnsave').html('Update');
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
                        debugger;
                        if (false === e.checkValidity()) {
                            validatePurchaseOrderForm();
                            e.classList.add("was-validated");
                        }
                        else {
                            
                            t.preventDefault();
                            t.stopPropagation();
                            e.classList.remove("was-validated");
                        



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

        // Initialize DataTable
            
        // Set focus order: after Bank, focus Regrigerant, then CompressorWarranty
        //$('#BankID').on('select2:close', function() {
        //    $('#RegrigerantID').select2('open');
        //});
        //$('#RegrigerantID').on('select2:close', function() {
        //    $('#CompressorWarrantyID').select2('open');
        //});
        // On edit, set value for Regrigerant and CompressorWarranty
        //if (typeof model !== 'undefined' && model.po) {
        //    alert(model.po.UnitWarrantyID);
        //    if (model.po.Regrigerant) {
        //        var regOption = new Option(model.po.RegrigerantText, model.po.Regrigerant, true, true);
        //        $('#RegrigerantID').append(regOption).trigger('change');
        //    }
        //    if (model.po.CompressorWarranty) {
        //        var compOption = new Option(model.po.CompressorWarrantyText, model.po.CompressorWarranty, true, true);
        //        $('#CompressorWarrantyID').append(compOption).trigger('change');
        //    }
         
        //    if (model.po.UnitWarrantyID) {
        //        var compOption = new Option(model.po.UnitWarrantyText, model.po.UnitWarrantyID, true, true);
        //        $('#UnitWarrantyID').append(compOption).trigger('change');
        //    }
        //}


        $('#btnaddestimation').click(function () {
            var _jobID = $('#ProjectNo').val();
            var jobno = $('#ProjectNo').select2('data')[0]?.text;

            $.ajax({
                type: "POST",
                url: '/PurchaseOrder/GetJobEquipmentDetail/',
                datatype: "html",
                data: { JobID: _jobID },
                success: function (data) {
                    $("#estimationdetailContainer").html(data);
                    $('#divEstimationNo').html('Project No.' + jobno);
                    $('#estimationdetailpopup').modal('show');
                }
            });
        })
        $('#flexSwitchCheckChecked').change(function () {
            if ($('#flexSwitchCheckChecked').prop('checked')) {
                $('#lblflexSwitch').html('Quotation by Estimation');
                $('#divestimationqutation').removeClass('d-xl-none');
                $('#divdirectquotation').addClass('d-xl-none');
            }
            else {
                $('#lblflexSwitch').html('Direct Quotation');
                $('#divestimationqutation').addClass('d-xl-none');
                $('#divdirectquotation').removeClass('d-xl-none');
            }

        });



        $('#btnaddApproval').click(function () {
            $('#approvalpopup').modal('show');            
        })

         //GetSalesOrderNo
        
        //if ($('#Id').val() == 0) {
        //    var _jobids = "";
        //    var maxcount=$('#PurchaseOrderEquipmentTables > tbody >tr').length;
        //    var idtext = 'QtxtAmount_';
        //    $('[id^=' + idtext + ']').each(function (index, item) {
        //        if ($('#QDeleted_' + index).val() != 'true') {
                  
        //            _jobids = _jobids + "," + $('#QJobHandOverID_' + index).val();
        //        }

        //        if (maxcount == (index + 1))
        //        {
        //            if (_jobids != '') {
        //                $.ajax({
        //                    type: "GET",
        //                    url: '/PurchaseOrder/GetSalesOrderNo/',
        //                    datatype: "json",
        //                    data: { JobIDs: _jobids },
        //                    success: function (data) {
        //                        //alert(data);
        //                        $('#po_SONoRef').val(data);
        //                    }
        //                });
        //            }

        //        }

        //    });
           
        //}

    });
})(jQuery)
