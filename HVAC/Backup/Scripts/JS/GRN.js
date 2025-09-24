

function saveGRN() {
    debugger;
    var GRN = {
        GRNID: $('#go_GRNID').val(),
        GRNNO: $('#go_GRNNO').val(),
        GRNDATE: $('#GrnDate').val(),
        SupplierID: $("#SupplierID").val(),
        PURCHASEORDERID: $('#PURCHASEORDERID').val(),
        EmployeeID: $("#EmployeeID").val(),
        Remarks: $("#go_Remarks").val()
    };
    var itemcount = $('#equipmentTable > tbody > tr').length;
    var quotationdetails = [];
    for (i = 0; i < itemcount; i++) {
        var quotationdetail = {
            GRNDetailID: $('#QGRNDetailID_' + i).val(),
            EquipmentID: $('#QEquipmentID_' + i).val(),
            EquipmentTypeID: $('#QEquipmentTypeID_' + i).val(),
            JobHandOverID: $('#QJobHandOverID_' + i).val(),
            ProjectNo: $('#QProjectNo_' + i).val(),
            Description: $('#QtxtDescription_' + i).val(),
            Model: $('#QtxtModel_' + i).val(),
            ItemUnitID: $('#QUnitID_' + i).val(),
            Quantity: $('#QtxtQty_' + i).val(),
            Rate: parsenumeric($('#QtxtUnitRate_' + i)),
            Amount: parsenumeric($('#QtxtAmount_' + i)),
            Checked: $('#chk_' + i).prop('checked')            
        }

        quotationdetails.push(quotationdetail);


        if (itemcount == (i + 1)) {
            $.ajax({
                url: '/GRN/SaveGRN',
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({
                    go: GRN,
                    gRNDetails: quotationdetails
                }),
                success: function (response) {
                    if (response.success) {
                        Swal.fire({
                            title: "Success!",
                            text: response.message || "Data saved successfully.",
                            icon: "success",
                            confirmButtonColor: "#34c38f"
                        }).then(() => {
                            window.location.href = "/GRN/Create/" + response.GRNID;
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
function calculateValue() {
    let unitPrice = parseFloat($('#GtxtUnitPrice').val()) || 0;
    let qty = parseFloat($('#GtxtQty').val()) || 0;
    let value = unitPrice * qty;

    $('#GtxtValue').val(value.toFixed(2));
    $('#GtxtFValue').val(value.toFixed(2)); // If needed for internal logic
    $('#GtxtLValue').val(value.toFixed(2));
}
 
 
(function ($) {

    'use strict';
    function initformControl() {
         
         
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
                            e.classList.add("was-validated");
                        }
                        else {

                            t.preventDefault();
                            t.stopPropagation();
                            e.classList.remove("was-validated");

                            saveGRN();



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
        var hdnid = $('#go_GRNID').val();
        if (hdnid > 0) {
            $('#btnsave').html('Update');
            let empTerm = new Option(masterDropDownList.EmployeeText, goModel.EmployeeID, true, true);
            $('#EmployeeID').append(empTerm).trigger('change');

            let purchaseTerm = new Option(masterDropDownList.PurchaseOrderText, goModel.PURCHASEORDERID, true, true);
            $('#PURCHASEORDERID').append(purchaseTerm).trigger('change');

            let Supp = new Option(masterDropDownList.SupplierText, goModel.SupplierID, true, true);
            $('#SupplierID').append(Supp).trigger('change');
        }

        $('#GrnDate').datepicker({
            dateFormat: 'dd-mm-yy',
            changeMonth: true,
            changeYear: true
        }).on('changeDate', function (e) {
            $(this).datepicker('hide');
        });


        $('#btnAddGRNDetails').click(function () {
            var editIndex = $("#hdnHidden").val();
            if (editIndex != '') {
                grnDetailsArray[editIndex].EquipmentTypeID = $('#EquipmentName').val();
                grnDetailsArray[editIndex].Model = $('#GtxtModel').val();
                grnDetailsArray[editIndex].Qty = parseInt($('#GtxtQty').val());
                grnDetailsArray[editIndex].UnitPrice = $('#GtxtUnitPrice').val();
                grnDetailsArray[editIndex].Value = $('#GtxtValue').val();
            } else {
                var data = {
                    EquipmentTypeID: $('#EquipmentName').val(),
                    Model: $('#GtxtModel').val(),
                    Qty: parseInt($('#GtxtQty').val()),
                    UnitPrice: $('#GtxtUnitPrice').val(),
                    Value: $('#GtxtValue').val(),
                };
                grnDetailsArray.push(data);
            }

            renderGRNDetailsTable();
            clearFormGrnDetails();

        });


        $('#GtxtUnitPrice, #GtxtQty').on('input', calculateValue);



        $('#SupplierID').select2({
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

        // When SupplierID changes, clear and enable/disable PURCHASEORDERID
        $('#SupplierID').on('change', function () {
            var supplierId = $(this).val();
            $('#PURCHASEORDERID').val(null).trigger('change');
            if (supplierId) {
                $('#PURCHASEORDERID').prop('disabled', false);
                $('#PURCHASEORDERID').select2({
                    placeholder: 'Select Purchase Order ID',
                    allowClear: true,
                    minimumInputLength: 0,
                    ajax: {
                        url: '/GRN/GetPurchaseOrderID',
                        dataType: 'json',
                        delay: 250,
                        data: function (params) {
                            var supplierId = $('#SupplierID').val();
                            if (!supplierId) {
                                // If no supplier selected, don't send request
                                return false;
                            }
                            return {
                                term: '',
                                supplierId: supplierId
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

            } else {
                $('#PURCHASEORDERID').prop('disabled', true);
            }
        });


        $('#EmployeeID').select2({
            placeholder: 'Select an Employee',
            allowClear: false,
            minimumInputLength: 1,
            ajax: {
                url: '/GRN/GetEmployeeName',
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

        $('#PURCHASEORDERID').select2({
            placeholder: 'Select Purchase Order ID',
            allowClear: true,
            minimumInputLength: 1,
            ajax: {
                url: '/GRN/GetPurchaseOrderID',
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

        $('#btnShowEquipment').click(function () {
            var purchaseOrderId = $('#PURCHASEORDERID').val();
            if (!purchaseOrderId) {
                Swal.fire('Please select a Purchase Order first.');
                return;
            }
            $.ajax({
                url: '/GRN/GetPurchaseOrderEquipment',
                type: 'Post',
                datatype: "html",
                data: { purchaseOrderId: purchaseOrderId, GRNID: 0 },
                success: function (data) {
                    $('#GRNContainer').html(data);

                    //var tbody = $('#equipmentTable tbody');
                    //tbody.empty();
                    //if (data && data.length > 0) {
                    //    data.forEach(function (item, idx) {
                    //        var row = '<tr>' +
                    //            '<td><input type="checkbox" class="equipment-checkbox" data-index="' + idx + '" /></td>' +
                    //            '<td>' + (item.Equipment || '') + '</td>' +
                    //            '<td>' + (item.Qty || '') + '</td>' +
                    //            '<td>' + (item.UnitPrice || '') + '</td>' +
                    //            '<td>' + (item.Value || '') + '</td>' +
                    //            '<td>' + (item.Model || '') + '</td>' +
                    //            '<td>' + (item.Remarks || '') + '</td>' +
                    //            '</tr>';
                    //        tbody.append(row);
                    //    });
                    // Store data for later use
                    //  $('#equipmentModal').data('equipmentList', data);
                    //} else {
                    //    tbody.append('<tr><td colspan="7" class="text-center">No equipment found for this Purchase Order.</td></tr>');
                    //}
                    //var modal = new bootstrap.Modal(document.getElementById('equipmentModal'));
                    //modal.show();
                },
                error: function () {
                    Swal.fire('Error loading equipment data.');
                }
            });
        });


    })


})(jQuery)