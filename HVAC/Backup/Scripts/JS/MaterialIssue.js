

function saveMaterialIssue() {
    debugger;
   
    var materialIssue = {
        MIssueID: $('#mi_MIssueID').val(),
        IssueDate: $('#IssueDate').val(),
        IssueNo: $('#mi_IssueNo').val(),
        JobHandOverID: $("#drpProjectNo").val(),
        RequestID: $("#drpRequestID").val(),
        RequestedBy: $("#mi_RequestedBy").val(),
        IssuedBy: $("#drpIssuedBy").val(),
        ApprovedBy: $("#drpApprovedBy").val(),
        Remarks: $("#mi_Remarks").val(),
        
    };
    var itemcount = $('#equipmentTable > tbody > tr').length;
    var quotationdetails = [];
    for (i = 0; i < itemcount; i++) {
        var quotationdetail = {
            IssueDetailID: $('#QIssueDetailID_' + i).val(),
            EquipmentID: $('#QEquipmentID_' + i).val(),
            EquipmentTypeID: $('#QEquipmentTypeID_' + i).val(),
            Description: $('#QtxtDescription_' + i).val(),
            Model: $('#QtxtModel_' + i).val(),
            ItemUnitID: $('#QUnitID_' + i).val(),
            Qty: $('#QtxtQty_' + i).val(),
            Checked: $('#chk_' + i).prop('checked')
        }

        quotationdetails.push(quotationdetail);
    }
        $.ajax({
            url: '/MaterialIssue/SaveMaterialIssue',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({
                mi: materialIssue,
                miDetails: quotationdetails
            }),
            success: function (response) {
                if (response.success) {
                    Swal.fire({
                        title: "Success!",
                        text: response.message || "Data saved successfully.",
                        icon: "success",
                        confirmButtonColor: "#34c38f"
                    }).then(function () {
                        window.location.href = '/MaterialIssue/Index';
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

                            saveMaterialIssue();



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
      
        $('#IssueDate').datepicker({
            format: 'dd/mm/yyyy',
            autoclose: true,
            todayHighlight: true
        });
 

        $('#btnShowMRDetails').click(function () {
            var _RequestID = $('#drpRequestID').val();
            if (!_RequestID) {
                Swal.fire('Please select a Material Request!.');
                return;
            }
            $.ajax({
                url: '/MaterialIssue/GetRequestEquipment',
                type: 'Post',
                datatype: "html",
                data: { MRequestId: _RequestID, },
                success: function (data) {
                    $('#GRNContainer').html(data);

                },
                error: function () {
                    Swal.fire('Error loading equipment data.');
                }
            });
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
        $('#drpRequestID').select2({
            width: 'resolve', // respects width set in the HTML
            placeholder: 'Select Material Request',
            allowClear: true,
            minimumInputLength: 0, // Show results on open
            ajax: {
                url: '/MaterialIssue/GetMaterialRequest',
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

        // Trigger search on open to show first 5 records
        $('#drpRequestID').on('select2:open', function (e) {
            setTimeout(function () {
                $('.select2-search__field').trigger('input');
            }, 0);
        });

        $('#drpIssuedBy').select2({
            placeholder: 'Select Employee',
            allowClear: false,
            minimumInputLength: 0,
            ajax: {
                url: '/Enquiry/GetMasterDataList',
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    return {
                        MasterName:"Employee", term: params.term
                    };
                },
                processResults: function (response) {
                    return {
                        results: response.data.map(function (item) {
                            return { id: item.ID, text: item.Text };
                        })
                    };
                },
                cache: true
            }
        });


        $('#drpApprovedBy').select2({
            placeholder: 'Select Employee',
            allowClear: false,
            minimumInputLength: 0,
            ajax: {
                url: '/Enquiry/GetMasterDataList',
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    return {
                        MasterName: "Employee", term: params.term
                    };
                },
                processResults: function (response) {
                    return {
                        results: response.data.map(function (item) {
                            return { id: item.ID, text: item.Text };
                        })
                    };
                },
                cache: true
            }
        });

        //GetMaterialRequestedBy
        $('#drpRequestID').change(function () {

            $.ajax({
                type: "GET",
                url: '/MaterialIssue/GetMaterialRequestedBy',
                datatype: "Json",
                data: { RequestId: $('#drpRequestID').val() },
                success: function (response) {

                    $('#RequestedByName').val(response.EmployeeName);
                    $('#mi_RequestedBy').val(response.EmployeeID);
                }
            })
        })

        var projectOption = new Option($('#ProjectNo').val(), $('#mi_JobHandOverID').val(), true, true);
        $('#drpProjectNo').append(projectOption).trigger('change');


        var materialRequestOption = new Option($('#RequestNo').val(), $('#RequestID').val(), true, true);
        $('#drpRequestID').append(materialRequestOption).trigger('change');

        var requestbyOption = new Option($('#RequestedByName').val(), $('#mi_RequestedBy').val(), true, true);
        $('#drpRequestedBy').append(requestbyOption).trigger('change');
        var IssueOption = new Option($('#IssuedByname').val(), $('#mi_IssuedBy').val(), true, true);
        $('#drpIssuedBy').append(IssueOption).trigger('change');


        var Approvedoption = new Option($('#ApprovedByName').val(), $('#mi_ApprovedBy').val(), true, true);
        $('#drpApprovedBy').append(Approvedoption).trigger('change');

 

      

    })
 



})(jQuery)