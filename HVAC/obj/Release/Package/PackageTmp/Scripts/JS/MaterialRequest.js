var _decimal = $('#hdncompanydecimal').val();
var editingIndex = null; // Track which row is being edited
var editRowIndex = null;

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
    var value = $('#QtxtValue').val();
    var qty = $('#QtxtQty').val();
    var rate = $('#QtxtRate').val();

    if (value == '')
        valu = 0;
    if (qty == '')
        qty = 0;
    if (parseFloat(value) > 0 && parseFloat(qty) > 0) {
        rate = parseFloat(value) / parseFloat(qty);
    }
    $('#QtxtRate').val(parseFloat(rate).toFixed(_decimal));

    //var value = parseFloat(rate) * parseFloat(qty);
    //$('#QtxtValue').val(parseFloat(value).toFixed(2));

}

function calculatevalue(index) {
    var rate = $('#QtxtRate_' + index).val();
    var qty = $('#QtxtQty_' + index).val();
    if (rate == '')
        rate = 0;
    if (qty == '')
        qty = 0;

    $('#QtxtRate').val(parseFloat(rate).toFixed(_decimal));
    var value = parseFloat(rate) * parseFloat(qty);
    $('#QtxtValue_' + index).val(parseFloat(value).toFixed(2));
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

function editGridRow(index) {
    editingIndex = index;
    var jobHandOverId = $('#QJobHandOverID_' + index).val();
    var projectNo = $('#QProjectNo_' + index).val();
    var projectName = $('#QProjectName_' + index).val();
    var equipmentName = $('#QEquipmentName_' + index).val();
    var equipmentId = $('#QEquipmentID_' + index).val();
    $('#Qty').val($('#QtxtQty_' + index).val());
    $('#Model').val($('#QtxtModel_' + index).val());
    $('#Description').val($('#QtxtDescription_' + index).val());
    $('#UnitTypeStockStatus').val($('#QUnitTypeStockStatus_' + index).val());
    $('#EstimationID').val($('#QtxtEstimationID_' + index).val());
    $('#QuotationID').val($('#QtxtQuotationID_' + index).val());
    setTimeout(function() {
        if (jobHandOverId && projectNo) {
            var projectOption = new Option(projectNo, jobHandOverId, true, true);
            $('#ProjectNo').empty().append(projectOption).val(jobHandOverId).trigger('change');
            $('#ProjectName').val(projectName);
            $('#JobHandOverID').val(jobHandOverId);
        }
        if (equipmentName && equipmentId) {
            $('#EquipmentName').empty();
            var equipmentOption = new Option(equipmentName, equipmentId, true, true);
            $('#EquipmentName').append(equipmentOption);
            $('#EquipmentName').val(equipmentId).trigger('change');
            $('#EquipmentID').val(equipmentId);
        }
    }, 100);
    $('#btnadd').html('<i class="dripicons-checkmark" style="font-size: 18px;"></i>');
    $('html, body').animate({
        scrollTop: $('#ProjectNo').offset().top - 100
    }, 500);
}

function EditDetailEntry(index) {
    editGridRow(index);
}

function DeleteDetailEntry(index) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#34c38f",
        cancelButtonColor: "#f46a6a",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $('#QDeleted_' + index).val('true');
            $('#quodtr_' + index).hide();
        }
    });
}

function AddEquipmentDetail(obj) {
    debugger;
    if ($('#ProjectNo').val() == '') {
        Swal.fire('Data Validation', 'Enter Project!');
        $('#ProjectNo').focus();
        return false;
    }
    else if ($('#EquipmentName').val() == '') {
        Swal.fire('Data Validation', 'Enter Equipment Name!');
        $('#EquipmentName').focus();
        return false;
    }
    else if ($('#Qty').val() == '' || $('#Qty').val() == '0') {
        Swal.fire('Data Validation', 'Enter Qty!');
        $('#Qty').focus();
        return false;
    }
    $(obj).attr('disabled', 'disabled');
    var projectData = $('#ProjectNo').select2('data')[0];
    var equipmententry = {
        MRequestDetailID: 0,
        EquipmentID: $('#EquipmentID').val(),
        JobHandOverID: $('#ProjectNo').val(),
        EquipmentName: $('#EquipmentName').select2('data')[0]?.text || $('#EquipmentName').val(),
        ProjectNo: projectData ? projectData.text : '',
        ProjectName: $('#ProjectName').val(),
        Qty: $('#Qty').val(),
        Model: $('#Model').val(),
        Description: $('#Description').val(),
        UnitTypeStockStatus: $('#UnitTypeStockStatus').val(),
        EstimationID: $('#EstimationID').val(),
        QuotationID: $('#QuotationID').val(),
        Deleted: false
    }
    console.log('equipmententry', equipmententry);
    var quotationdetails = [];
    var itemcount = $('#DetailTables > tbody > tr').length;
    for (i = 0; i < itemcount; i++) {
        var quotationdetail = {
            MRequestDetailID: $('#QMRequestDetailID_' + i).val(),
            EquipmentID: $('#QEquipmentID_' + i).val(),
            JobHandOverID: $('#QJobHandOverID_' + i).val(),
            EquipmentName: $('#QEquipmentName_' + i).val(),
            ProjectNo: $('#QProjectNo_' + i).val(),
            ProjectName: $('#QProjectName_' + i).val(),
            Qty: $('#QtxtQty_' + i).val(),
            Model: $('#QtxtModel_' + i).val(),
            Description: $('#QtxtDescription_' + i).val(),
            UnitTypeStockStatus: $('#QUnitTypeStockStatus_' + i).val(),
            EstimationID: $('#QtxtEstimationID_' + i).val(),
            QuotationID: $('#QtxtQuotationID_' + i).val(),
            Deleted: $('#QDeleted_' + i).val()
        }
        quotationdetails.push(quotationdetail);
    }
    $.ajax({
        type: "POST",
        url: '/MaterialRequest/AddEquipmentEntry/',
        datatype: "html",
        data: { invoice: equipmententry, index: (editingIndex !== null ? editingIndex : -1), Details: JSON.stringify(quotationdetails) },
        success: function (data) {
            $("#DetailContainer").html(data);
            $(obj).removeAttr('disabled');
            $('#ProjectNo').focus();
            // Clear form fields after successful add
            $('#EquipmentName').val('').trigger('change');
            $('#EquipmentID').val('');
            $('#Qty').val('');
            $('#Model').val('');
            $('#Description').val('');
            $('#UnitTypeStockStatus').val('');
            $('#EstimationID').val('');
            $('#QuotationID').val('');
            editingIndex = null; // Reset editing index
            // Reset button text
            $('#btnadd').html('<i class="dripicons-enter" style="font-size: 18px;"></i>');
        },
        error: function (xhr, status, error) {
            $(obj).removeAttr('disabled');
            Swal.fire('Error', 'Failed to add equipment detail!', 'error');
        }
    });
}

function SaveMaterialRequest() {
    debugger;
    var itemcount = $('#equipmentTable > tbody > tr').length;
    if (itemcount == 0) {
        return false;
    }
    var materialRequestDetails = [];
    for (i = 0; i < itemcount; i++) {
        
            var detail = {
                EquipmentID: $('#QEquipmentID_' + i).val(),
                QuotationID: $('#QQuotationID_' + i).val(),
                EstimationID: $('#QEstimationID_' + i).val(),
                EquipmentTypeID: $('#QEquipmentTypeID_' + i).val(),
                Description: $('#QtxtDescription_' + i).val(),
                Model: $('#QtxtModel_' + i).val(),
                ItemUnitID: $('#QUnitID_' + i).val(),
                Qty: $('#QtxtQty_' + i).val(),
                Checked: $('#chk_' + i).prop('checked')
            }
    materialRequestDetails.push(detail);
  }
    

    // Prepare main material request data
    var materialRequest = {
        MRequestID: $('#MRequestID').val() || 0,
        MRNo: $('#MRNo').val(),
        MRDate: $('#MRDate').val(),        
        RequestedBy: $('#RequestedBy').val(),      
        JobHandOverID: $('#drpProjectNo').val(),
        JobPurchaseOrderDetailID: $('#drpClientPO').val(),
        StoreKeeperID: $('#StoreKeeperID').val() ? parseInt($('#StoreKeeperID').val()) : null,
        Remarks: $('#Remarks').val()
    };

    // Disable save button
    $('#btnsave').attr('disabled', 'disabled');

    $.ajax({
        type: "POST",
        url: '/MaterialRequest/SaveMaterialRequest/',
        datatype: "json",
        data: { materialrequest: materialRequest, Details: JSON.stringify(materialRequestDetails) },
        success: function (response) {
            if (response.status == "ok") {
                Swal.fire("Save Status!", response.message, "success");
                setTimeout(function () {
                    window.location.href = "/MaterialRequest/Index";
                }, 1000);
            }
            else {
                $('#btnsave').removeAttr('disabled');
                Swal.fire("Save Status!", response.message, "warning");
            }
        },
        error: function (xhr, status, error) {
            $('#btnsave').removeAttr('disabled');
            Swal.fire('Error', 'Failed to save material request!', 'error');
        }
    });
}
 
 
 
 

 
 
(function ($) {

    'use strict';
    function initformControl() {
        //$('#MRDate').datepicker({
        //    dateFormat: 'dd-mm-yy',
        //    changeMonth: true,
        //    changeYear: true
        //}).on('changeDate', function (e) {
        //    $(this).datepicker('hide');
        //});

        $('#MRDate').datepicker({
            dateFormat: 'dd-mm-yy',   // jQuery UI date format
            showOnFocus: false,       // prevent popup on focus
            autoclose: true,
            todayHighlight: true,
            container: $('.docs-datepicker-container') // if needed
        }).on('blur change', function () {
            let val = $(this).val().trim();

            // ✅ Match dd-mm only (e.g. 15-07)
            if (/^\d{2}-\d{2}$/.test(val)) {
                let parts = val.split("-");
                let currentYear = new Date().getFullYear();
                let fullDate = `${parts[0]}-${parts[1]}-${currentYear}`;
                $(this).datepicker('setDate', fullDate);  // sets the corrected date
            }

            // ✅ Match dd-mm-yyyy → do nothing (already full)
            else if (/^\d{2}-\d{2}-\d{4}$/.test(val)) {
                // optional: validate
            }

            // ❌ Invalid format → clear or highlight
            else if (val !== "") {
                //alert("Invalid date format! Please enter dd-mm or dd-mm-yyyy");
                $(this).val(""); // clear invalid entry
            }
        });

        // ✅ ALT + ↓ opens datepicker manually
        $('#MRDate').on('keydown', function (e) {
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
        $('#btnShowMRDetails').click(function () {
            var _RequestID = $('#drpClientPO').val();
            if (!_RequestID) {
                Swal.fire('Please select a Client PO!.');
                return;
            }
            $.ajax({
                url: '/MaterialRequest/GetRequestEquipment',
                type: 'Post',
                datatype: "html",
                data: { ClientPOID: _RequestID, },
                success: function (data) {
                    $('#GRNContainer').html(data);

                },
                error: function () {
                    Swal.fire('Error loading equipment data.');
                }
            });
        });

        //GetProjectNo
        $('#drpProjectNo').select2({
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
        // Initialize Select2 for dropdowns
        $('#drpClientPO').select2({
            width: 'resolve', // respects width set in the HTML
            placeholder: 'Select Client PO',
            allowClear: true,
            minimumInputLength: 0, // Show results on open
            ajax: {
                url: '/MaterialRequest/GetClientPO',
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    return {
                        term: params.term || '',
                        projectId: $('#drpProjectNo').val() // send selected project ID
                    };
                },
                processResults: function (data) {
                    return {
                        results: data.map(function (item) {
                            return { id: item.ID, text: item.TermsText };
                        })
                    };
                },
                cache: true
            }
        });

        // Initialize Equipment Name with Select2 (like Estimation)
        $('.equipment-select').each(function () {
            var $select = $(this);
            
            $select.select2({
                placeholder: 'Select Equipment',
                allowClear: false,
                minLength: 1,
                ajax: {
                    url: '/MaterialRequest/GetEquipmentType',
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
                                return { id: item.ID, text: item.EquipmentName };
                            })
                        };
                    },
                    cache: true
                }
            });

            // On equipment select, fill EquipmentID
            $select.on('select2:select', function (e) {
                var data = e.params.data;
                $('#EquipmentID').val(data.id);
                // The equipment name is already set in the Select2 dropdown text
                // No need to set EquipmentName separately as it's the same field
            });
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

                            SaveMaterialRequest();

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

        var projectOption = new Option($('#ProjectNo').val(), $('#JobHandOverID').val(), true, true);
        $('#drpProjectNo').append(projectOption).trigger('change');


        var materialRequestOption = new Option($('#PONO').val(), $('#JobPurchaseOrderDetailID').val(), true, true);
        $('#drpClientPO').append(materialRequestOption).trigger('change');

        $('#MRDate').blur(function () {
            $('#drpProjectNo').select2('open');
        })
        $('#drpProjectNo').blur(function () {
            $('#drpClientPO').select2('open');
        })
        $('#MRDate').focus();
        $('#MRDate').data("manual-open", true).datepicker("hide");

    });

 

 

 