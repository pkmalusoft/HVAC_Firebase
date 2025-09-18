function calculateDueDays1() {
    var startDate = new Date($("#EnquiryDate").val());
    var endDate = new Date($("#EnquiryDueDate").val());

    if (!isNaN(startDate) && !isNaN(endDate)) {
        var timeDiff = endDate - startDate;
        var dueDays = timeDiff / (1000 * 3600 * 24); // Convert milliseconds to days
        $("#DueDays").val(dueDays);
    } else {
        $("#DueDays").val(""); // Clear if invalid dates
    }
}
function validateDates() {
    let startDateStr = $("#EnquiryDate").val();
    let endDateStr = $("#EnquiryDueDate").val();

    if (startDateStr && endDateStr) {
        let startDateParts = startDateStr.split("-");
        let endDateParts = endDateStr.split("-");

        let startDate = new Date(startDateParts[2], startDateParts[1] - 1, startDateParts[0]); // yyyy, mm (0-based), dd
        let endDate = new Date(endDateParts[2], endDateParts[1] - 1, endDateParts[0]);

        if (endDate <= startDate) {
            Swal.fire('Data Validation', 'Due Date should be greater than Enquiry date!');
            //$("#errorMsg").show();  // Show error message
            //$("#endDate").val("");  // Clear invalid end date
            //$("#dueDays").val("");  // Clear due days
        } else {
            //$("#errorMsg").hide();  // Hide error message
            //let dueDays = (endDate - startDate) / (1000 * 60 * 60 * 24); // Convert milliseconds to days
            //$("#dueDays").val(dueDays);
        }
    } else {
        //$("#dueDays").val(""); // Clear field if any date is missing
    }
}
function calculateDueDays() {
    let startDateStr = $("#EnquiryDate").val();
    let endDateStr = $("#EnquiryDueDate").val();

    if (startDateStr && endDateStr) {
        let startDateParts = startDateStr.split("-");
        let endDateParts = endDateStr.split("-");

        let startDate = new Date(startDateParts[2], startDateParts[1] - 1, startDateParts[0]); // yyyy, mm (0-based), dd
        let endDate = new Date(endDateParts[2], endDateParts[1] - 1, endDateParts[0]);

        let dueDays = (endDate - startDate) / (1000 * 60 * 60 * 24); // Convert milliseconds to days

        $("#DueDays").val(dueDays);
        $('#lblDueDays').html('Due Days :' +dueDays);
    } else {

        $('#lblDueDays').html('');
        $("#DueDays").val(""); // Clear field if any date is missing
    }
}
function ValidateEmail(inputText) {
    //var mailformat = /^\w+([\.-]?\w+)*@@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
    //if (inputText.match(mailformat)) {
    if (/^\w+([\.-]?\w+)*@@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(inputText)) {
        return true;
    }
    else {
        $('#msg1').html("Invalid Email address!");
        $('#Email').val('');
        $('#Email').focus();
        return false;
    }
}

function IsEmail(email) {
    var regex = /^([a-zA-Z0-9_\.\-\+])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    if (!regex.test(email)) {
        return false;
    } else {
        return true;
    }
}
function numberWithCommas(x) {
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}

 
function CheckProjectName() {
    
    
    if ($('#ProjectName').val().trim() != "") {
        $.ajax({
            type: "Get",
            url: "/Enquiry/CheckProjectExist",
            datatype: "Json",
            data: { ProjectName: $('#ProjectName').val() },
            success: function (data) {
                debugger;
                console.log(data);
                if (data == "true") {
                    Swal.fire("Data Validation", "Project Name already exists", "warning");
                     
                    $('#ProjectName').focus();
                    return false;
                }
                else {
                  
                    return true;
                }

            }
        });
    }
}


function SaveEnquiry() {
   
    debugger;
    var idtext = 'hdnClientId_';
    var entityitems = $('#drpEntityTypeIDs').val();

    $('#EntityTypeIDs').val($('#drpEntityTypeIDs').val());

    //if (entityitems == null) {
    //    Swal.fire('Data Validation', 'Select Entity', 'warning');
    //    return false;
    //}
    //var entitys = '';
    //$.each(entityitems, function (index, item) {
    //    if (item != '') {
    //        entitys = entitys + ',' + item;
    //    }
    //    $('#EntityTypeIDs').val(entitys);
    //});   

    //var employeeitems = $('#drpAssignedEmployees').val();
    //var employees = '';
    //if ($('#EnquiryStatusID option:selected').text() == 'Assigned') {
    //    if (employeeitems == null || employeeitems == '' || employeeitems == '[]') {
    //        Swal.fire('Data Validation', 'Select Employee', 'warning');
    //        return false;
    //    }
    //}
    //if (employeeitems != null) {

    //    $.each(employeeitems, function (index, item) {
    //        if (item != '') {
    //            employees = employees + ',' + item;
    //        }
    //        $('#AssignedEmployees').val(employees);
    //    });
    //}

    //var clientcount = 0;
    //$('[id^=' + idtext + ']').each(function (index, item) {
    //    var _deleted = $('#hdnclientdeleted_' + index).val();

    //    var _clientid = $('#hdnClientId_' + index).val();
    //    if (_deleted !== "true") {
    //        clientcount = clientcount + 1;
    //    }        
    //})

    //if (clientcount == 0) {
    //    Swal.fire('Data Validation', 'Add Client', 'warning');
    //    return false;
    //}
    //if (employeeitems == null)
    //    employees = '';
    $('#btnsave').attr('disabled', 'disabled');

    
    var id = 0;
    if ($('#EnquiryID').val() > 0) {
        id = $('#EnquiryID').val();


    }
    var protypeobj = {
        EnquiryID: $('#EnquiryID').val(),
        EnquiryDate: $('#EnquiryDate').val(),
        EnquiryNo: $('#EnquiryNo').val(),

         ProjectName: $('#ProjectName').val(),
        ProjectDescription: $('#ProjectDescription').val(),
        DueDate: $('#EnquiryDueDate').val(),
        DueDays: $('#DueDays').val(),

        EnquiryStageID: $('#EnquiryStageID').val(),
        EntityTypeIDs: $('#EntityTypeIDs').val(),
        AssignedEmployees: $('#AssignedEmployees').val(),
        PriorityID: $('#PriorityID').val(),

        VerticalsID: $('#VerticalsID').val(),
        EnquiryStatusID: $('#EnquiryStatusID').val(),
        EnquirySourceID: $('#EnquirySourceID').val(),
        BuildingTypeID: $('#BuildingTypeID').val(),
        CityID: $('#CityID').val(),
        CityName: $('#CityName').val(),
        CountryID: $('#CountryID').val(),
        CountryName: $('#CountryName').val(),
        VerticalsID: $('#VerticalsID').val(),
        PriorityID: $('#PriorityID').val(),
        ProjectPrefix: $('#ProjectPrefix').val(),
        EnquiryTypeID: $('#EnquiryTypeID').val(),
        ClientID : $('#drpClientID').val()

    }
    debugger;
    $.ajax({
        type: "POST",
        url: '/Enquiry/SaveEnquiry',
        datatype: "html",
        data: { ProType: protypeobj, Details: "" },
        success: function (response) {
            if (response.status == "OK") {
                Swal.fire("Save Status!", response.message, "success");
                setTimeout(function () {
                    window.location.href = "/Enquiry/Create?id=" + response.EnquiryID;
                }, 100)
                $('#btnsave').removeAttr('disabled');
            }
            else {
                Swal.fire("Save Status!", response.message, "warning");
                $('#btnsave').removeAttr('disabled');
            }


        }
    });

    //var itemcount = $('#tbodyclient > tr').length;
    //var clients = [];
    //if (itemcount == 0) {

    //    $.ajax({
    //        type: "POST",
    //        url: '/Enquiry/SaveEnquiry',
    //        datatype: "html",
    //        data: { ProType: protypeobj, Details: "" },
    //        success: function (response) {
    //            if (response.status == "OK") {
    //                Swal.fire("Save Status!", response.message, "success");
    //                setTimeout(function () {
    //                    window.location.href = "/Enquiry/Create?id=" + response.EnquiryID;
    //                }, 100)

    //            }
    //            else {
    //                Swal.fire("Save Status!", response.message, "warning");
    //            }


    //        }
    //    });
    //}
    //else {
    //    $('[id^=' + idtext + ']').each(function (index, item) {
    //        var _deleted = $('#hdnclientdeleted_' + index).val();
          
    //            var _clientid = $('#hdnClientId_' + index).val();
    //        var _clientdata = {
    //            ClientID: _clientid,
    //            Deleted: _deleted,
    //            ClientType : $('#hdnclienttype_' + index).val() 
    //        }
    //            clients.push(_clientdata);
            

    //        if ((index + 1) == itemcount) {
    //            $.ajax({
    //                type: "POST",
    //                url: '/Enquiry/SaveEnquiry',
    //                datatype: "Json",
    //                data: { ProType: protypeobj, Details: JSON.stringify(clients) },
    //                success: function (response) {
    //                    if (response.status == "OK") {

    //                        Swal.fire("Save Status", response.message, "success");
    //                        setTimeout(function () {
    //                            if ($('#EnquiryID').val() == 0) {
    //                                location.href = '/Enquiry/Create?id=' + response.EnquiryID;
    //                            }
    //                            else {
    //                                location.href = '/Enquiry/Index';
    //                            }

    //                        }, 200)

    //                    }
    //                    else {
    //                        $('#btnsave').removeAttr('disabled');
    //                        Swal.fire("Save Status", response.message, "error");
    //                        return false;
    //                    }
    //                }
    //            });
    //        }

    //    });

    //}
    
   
}
//Client entry start
function ShowClientEntry() {

    debugger;
    $.ajax({
        type: 'POST',
        url: '/Enquiry/ShowClientEntry',
        datatype: "html",
        data: {
            FieldName: 's'
        },
        success: function (data) {
            $("#customerContainer").html(data);
            //target
            $('#customerpopup').modal('show');
            $('#spancusterror').html("");
            setTimeout(function () {
                $('#ClientName').focus();
            }, 200)

        }
    });
}


function deleteclientItemtrans(obj) {
    var id = $(obj).attr('id').split('_')[1];
    $(obj).parent().parent().parent().addClass('hide');
    $('#hdnclientdeleted_' + id).val('true');

    //calculatenettotal();
    //resetboxno();

}

function AddClient() {
    var selectedClientType = $('#drpClientType').val();

    if ($('#drpClientID1').val() == null || $('#drpClientID1').val() == '') {
        Swal.fire({
            title: 'Data Validation',
            text: 'Select Client!',
            icon: 'warning',
            timer: 2000, // 20 seconds = 20000 ms
            timerProgressBar: true,
            didOpen: () => {
                Swal.showLoading(); // Optional: show spinner while waiting
            }
        });
        return;
    }
    var protypeobj = {
        EnquiryClientID: 0,
        ClientID: $('#drpClientID1').val(),
        ClientType: $('#drpClientType').val(),
        EnquiryID: $('#EnquiryID').val(),
        EnquiryNo: $('#EnquiryNo').val()
    }
    // var clientid = $('#drpClientID1').val();

    $.ajax({
        type: 'POST',
        url: '/Enquiry/SaveEnquiryClient',
        dataType: "json",
        data: {
            data: protypeobj
        },
        success: function (response) {
            debugger;
            if (response.status == "OK") {
                $('#customerpopup').modal('hide');
                Swal.fire({
                    title: response.message,
                    icon: 'success',
                    timer: 2000, // 20 seconds = 20000 ms
                    //timerProgressBar: true,
                    //didOpen: () => {
                    //    Swal.showLoading(); // Optional: show spinner while waiting
                    //}
                });
                $.ajax({
                    type: 'POST',
                    url: '/Enquiry/ShowClientList',
                    datatype: "html",
                    data: {
                        EnquiryID: $('#EnquiryID').val()
                    },
                    success: function (data) {
                        $("#divclient").html(data);
                    }
                });
            }
            else {
                $('#customerpopup').modal('hide');
                Swal.fire('Data Validation', response.message, 'warning');                    
            }
        }
    });    
}
function SaveClientEntry() {
    if ($('#cClientName').val().trim() == '') {
        $('#spancusterror').html("Enter Name!");
        $('#cClientName').focus();
        return;
    }
    //else if ($('#cContactNo').val().trim() == '') {
    //    $('#spancusterror').html("Enter Customer Phone No.!");
    //    $('#cContactNo').focus();
    //    return;
    //}
    else if ($('#cCityName').val().trim() == '') {
        $('#spancusterror').html("Enter City Name!");
        $('#cCityName').focus();
        return;
    }
    else if ($('#cCountryName').val().trim() == '') {
        $('#spancusterror').html("Enter Country Name!");
        $('#cCountryName').focus();
        return;
    }
    $('#spancusterror').html("");

    var cust = {
        ClientName: $('#cClientName').val(),
        Address1: $('#cAddress1').val(),
        Address2: $('#cAddress2').val(),
        Address3: $('#cAddress3').val(),
        LocationName: $('#cLocationName').val(),
        CityID: $('#cCityID').val(),
        CountryID: $('#cCountryID').val(),
        ContactName: $('#cContactName').val(),
        ContactNo: $('#cContactNo').val(),
        Email :$('#cEmail').val(),
        ClientType: $('#CustomerType1').val(),
        ClientPrefix: $('#cClientPrefix').val(),
        EnquiryID : $('#EnquiryID').val()
    }

    $.ajax({
        type: 'POST',
        url: '/Enquiry/SaveClientEntry',
        datatype: "json",
        data: cust,
        success: function (response) {
            debugger;
            if (response.status == 'Ok') {

                $('#ClientName').val(response.data.CustomerName);
                $('#customer').attr('customername', response.data.CustomerName)
                $('#customerpopup').modal('hide');
                $('#customer').trigger('change');
                Swal.fire("Save Status!", response.message, "success");
                $('#CustomerName').val(response.data.CustomerName);
                $('#CustomerID').val(response.data.CustomerID);
                $('#Mobile1').val(response.data.Mobile);
                $('#CusAddress').val(response.data.Address1);
                $('#Location').val(response.data.LocationName);
                $('#City').val(response.data.CityName);
                $('#Country').val(response.data.CountryName);




                //$.notify(response.message, "success");

            }
            else {
                //$('#customerpopup').modal('hide');
                //$('#customer').val(response.data.CustomerName);
                //$.notify(response.message, "error");
                Swal.fire("Save Status!", response.message, "error");
                $('#CustomerName').focus();
            }
        }
    });

   
}

// end Save Client

//document entry start
function ShowDocumentEntry() {

    debugger;
    $.ajax({
        type: 'POST',
        url: '/Enquiry/ShowDocumentEntry',
        datatype: "html",
        data: {
            id: 0, EnquiryID: $('#EnquiryID').val()
        },
        success: function (data) {
            $("#documentContainer").html(data);
            //target
            $('#documentpopup').modal('show');
            $('#spandocerror').html("");
            setTimeout(function () {
                $('#DocumentType').focus();
            }, 200)

        }
    });
}


//equipment entry start
function ShowEquipmentEntry() {
    debugger;
    $.ajax({
        type: 'POST',
        url: '/Enquiry/ShowEquipmentEntry',
        datatype: "html",
        data: {
            id: 0, EnquiryID: $('#EnquiryID').val()
        },
        success: function (data) {
            $("#equipmentContainer").html(data);
            //target
            $('#equipmentpopup').modal('show');
            $('#spanenquiryerror').html("");
            setTimeout(function () {
                $('#EquipmentCategoryID').focus();
            }, 200)

        }
    });
}

//Show engineer entry

function ShowEngineerEntry() {
    debugger;
    $('#engineerpopup').modal('show');
    //$('#spanenquiryerror').html("");
    setTimeout(function () {
        $('#drpAssignedEmployees').focus();
    }, 200)

    //$.ajax({
    //    type: 'POST',
    //    url: '/Enquiry/ShowEngineerEntry',
    //    datatype: "html",
    //    data: {
    //        id: 0, EnquiryID: $('#EnquiryID').val()
    //    },
    //    success: function (data) {
    //        //$("#engineerContainer").html(data);
    //        //target
            

    //    }
    //});
}

function SaveEngineerEntry() {
    debugger;
    $('#btnSaveEngineer').attr('disabled', 'disabled');
    debugger;       
    var engineerentry = {
        ID: 0,
        EnquiryID: $('#EnquiryID').val(),
        EmployeeID: $('#drpAssignedEmployees').val(),
        EnquiryNo :$('#EnquiryNo').val()
    }

    $.ajax({
        type: "POST",
        url: '/Enquiry/SaveEnquiryEmployee/',
        datatype: "json",
        data: engineerentry,
        success: function (response) {
            debugger;
            if (response.status == "OK") {

                Swal.fire({
                    title: "Save Status",
                    text: response.message,
                    icon: "success",
                    timer: 200
                    //showCancelButton: true,
                    //confirmButtonText: "Yes, delete it!"
                });
                $('#engineerpopup').modal('hide');
                $('#btnSaveEngineer').removeAttr('disabled');

                $.ajax({
                    type: 'POST',
                    url: '/Enquiry/ShowEngineer',
                    datatype: "html",
                    data: {
                        EnquiryID: $('#EnquiryID').val()
                    },
                    success: function (data) {
                        $("#divengineer").html(data);                        
                    }
                });
                 
            }
            else {
                Swal.fire({
                    title: "Save Status",
                    text: response.message,
                    icon: "warning",
                    timer: 200
                    //showCancelButton: true,
                    //confirmButtonText: "Yes, delete it!"
                });

            }


        }
    });
}

function DeleteEnquiryEmployee(ID, index1) {
    setdocumentrowactive(index1);
    Swal.fire({ title: "Are you sure?", text: "You won't be able to revert this!", icon: "warning", showCancelButton: !0, confirmButtonColor: "#34c38f", cancelButtonColor: "#f46a6a", confirmButtonText: "Yes, delete it!" }).then(
        function (t) {
            if (t.value) {
                $.ajax({
                    type: "POST",
                    url: '/Enquiry/DeleteEmployeeEnquiry',
                    datatype: "html",
                    data: {
                        'ID': ID, 'EnquiryID': $('#EnquiryID').val()
                    },
                    success: function (data) {
                                                

                        if (data.Status == 'OK') {
                            Swal.fire("Deleted!", "Engineer UnAssigned.", "success");
                            $.ajax({
                                type: 'POST',
                                url: '/Enquiry/ShowEngineer',
                                datatype: "html",
                                data: {
                                    EnquiryID: $('#EnquiryID').val()
                                },
                                success: function (data) {
                                    $("#divengineer").html(data);
                                }
                            });
                            
                        }
                        else {
                            Swal.fire("Deleted!", "Engineer added Quotation,could not unassign!", "success");
                        }


                    }
                });
            }
        });


}
(function ($) {

    'use strict';
    function initformControl() {
        $('#EnquiryDate').datepicker({
            dateFormat: 'dd-mm-yy',
            onClose: function (dateText, inst) {
                if (/^\d{2}-\d{2}$/.test(dateText)) {
                    let parts = dateText.split("-");
                    let currentYear = new Date().getFullYear();
                    let fullDate = `${parts[0]}-${parts[1]}-${currentYear}`;
                    $(this).datepicker('setDate', fullDate);
                }
            },
            container: $(this).closest('.docs-datepicker').find('.docs-datepicker-container'),
            showOnFocus: false, // prevents auto popup on input focus
            todayHighlight: true,
            //changeMonth: true,
            //changeYear: true,
            autoclose: true,
        }).on('show', function (e) {
            // Optional: blur input when manually triggered
            $(this).blur();
        });


        $('#EnquiryDueDate').datepicker({
            dateFormat: 'dd-mm-yy',
            onClose: function (dateText, inst) {
                if (/^\d{2}-\d{2}$/.test(dateText)) {
                    let parts = dateText.split("-");
                    let currentYear = new Date().getFullYear();
                    let fullDate = `${parts[0]}-${parts[1]}-${currentYear}`;
                    $(this).datepicker('setDate', fullDate);
                }
            },
            autoclose: true,
            showOnFocus: false, // prevents auto popup on input focus
            todayHighlight: true,
            container: $(this).closest('.docs-datepicker').find('.docs-datepicker-container')
        }).on('show', function (e) {
            // Optional: blur input when manually triggered
            $(this).blur();
        });

      
        
        // Handle calendar icon click
        $('.docs-datepicker-trigger').on('click', function () {
            const $input = $(this).closest('.input-group').find('.docs-date');
            $input.datepicker('show');
        });
        $("#EnquiryDate, #EnquiryDueDate").on("change", function () {
            calculateDueDays();
        });

        $('#CountryName').autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/LocationMaster/GetCountryName',
                    data: {
                        term: request.term
                    },
                    dataType: "json",
                    type: "GET",
                    success: function (data) {
                        response($.map(data, function (val, item) {
                            return {
                                label: val.CountryName,
                                value: val.Id
                            }
                        }))
                    }
                });
            },
            minLength: 1,
            autoFocus: false,
            select: function (event, ui) {
                event.preventDefault();
                $('#CountryID').val(ui.item.value);
                $('#CountryName').val(ui.item.label);
                return false;
            },
            focus: function (event, ui) {
                $('#CountryID').val(ui.item.value);
                $('#CountryName').val(ui.item.label);

                return false;
            }
        });
        $('#CityName').autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: "/LocationMaster/GetCityName",
                    data: { term: request.term },
                    dataType: "json",
                    type: "GET",
                    success: function (data) {
                        response($.map(data, function (val, item) {
                            return {
                                label: val.CityName,
                                value: val.Id,
                                Country: val.CountryName,
                                CountryID: val.CountryID
                            }
                        }))
                    }
                });
            },
            minLength: 1,
            autoFocus: false,
            select: function (event, ui) {
                event.preventDefault();
                $('#CityID').val(ui.item.value);
                $('#CountryID').val(ui.item.CountryID);
                $('#CityName').val(ui.item.label);
                $('#CountryName').val(ui.item.Country);
                return false;
            },
            focus: function (event, ui) {
                $('#CityID').val(ui.item.value);
                $('#CountryID').val(ui.item.CountryID);
                $('#CityName').val(ui.item.label);
                $('#CountryName').val(ui.item.Country);
                return false;
            }
        });

        $('#ProjectPrefix').autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: "/Enquiry/GetProjectPrefix",
                    data: { term: request.term },
                    dataType: "json",
                    type: "GET",
                    success: function (data) {
                        response($.map(data, function (val, item) {
                            return {
                                label: val.ProjectfixName,
                                value: val.ID                                 
                            }
                        }))
                    }
                });
            },
            minLength: 1,
            autoFocus: false,
            select: function (event, ui) {
                event.preventDefault();
                $('#ProjectPrefix').val(ui.item.label);                
                return false;
            },
            focus: function (event, ui) {
                $('#ProjectPrefix').val(ui.item.label);
                return false;
            }
        });
        
       

    }
    function init() {
        initformControl();
        //for client binding
        var suppressOpenOnClear = false;
        //$('.enquiry-client').each(function () {
        //    var $select = $(this);
           
        //    // Handle the clear event
        //    $select.on('select2:select2-selection__arrow', function () {
        //        suppressOpenOnClear = true;

        //        // Remove focus from both the hidden <select> and visible Select2 box
        //        setTimeout(() => {
        //            $select.blur();
        //            $select.next('.select2-container').find('.select2-selection').blur();
        //        }, 0);
        //    });

        //    // Prevent dropdown from reopening immediately after clear
        //    $select.on('select2:opening', function (e) {
        //        if (suppressOpenOnClear) {
        //            e.preventDefault(); // Stop it from opening
        //            suppressOpenOnClear = false;
        //        }
        //    });
        //    var defaultOption = new Option($select.attr('boxname'), $select.attr('boxid'), true, true);
        //    $select.append(defaultOption);
        //    //// Append it to the select
        //    $select.append('<option id="" value=""></option>').trigger('change');
        //    $select.select2({
        //        width: 'resolve', // respects width set in the HTML
        //        placeholder: 'Select a Client',               
        //        allowClear: false,
        //        minLength: 1,
        //        ajax: {
        //            url: '/Enquiry/GetClientName',
        //            dataType: 'json',
        //            delay: 250,
        //            data: function (params) {
        //                // var category = $select.closest('tr').find('.category').val();
        //                return {
        //                    term: params.term,
        //                    ClientType: ''

        //                };
        //            },
        //            processResults: function (data) {
        //                return {
        //                    results: data.map(function (item) {
        //                        return { id: item.ClientID, text: item.ClientName };
        //                    })
        //                };
        //            },
        //            cache: true
        //        }
        //    });



        //});
        setTimeout(function () {
            if ($('#EnquiryID').val() > 0) {
                $('#btnsave').val('Update');

                // Prevent auto-popup on focus
                $(document).on('focus', '.docs-date', function (e) {
                    $(this).datepicker('hide');  // Hide if it tries to open
                });
                $("#EnquiryDate").focus();
                $("#EnquiryDate").datepicker('hide');  // Hide if it tries to open
            }
            else {


            }
        }, 100)
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
                            SaveEnquiry();

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
        
      //for entitty type binding
        $.ajax({
            type: "GET",
            url: '/Enquiry/GetMasterDataList/',
            data: {
                'MasterName':'EntityType','term':""
            },
            success: function (response) {
                debugger;
                if (response.Status == "ok") {
                    console.log(response.data);
                    var data = response.data;
                    var selectedjobid = [];
                    if ($('#EntityTypeIDs').val() != '') {
                        selectedjobid = $('#EntityTypeIDs').val().split(',');
                    }
                    $.each(data, function (index, item) {
                        var value = data[index].ID;
                        var label = data[index].Text;
                             

                        var selected = false;
                        $.each(selectedjobid, function (val1, item1) {
                            if (value == item1)
                                selected = true;
                        });
                        if (selected == true) {
                            var html = '<option selected value="' + value + '">' + label + '</option>';
                            $('#drpEntityTypeIDs').append(html);
                        }
                        else {
                            var html = '<option value="' + value + '">' + label + '</option>';
                            $('#drpEntityTypeIDs').append(html);
                        }
                    })
                    $('#drpEntityTypeIDs').select2();
                     
                } else {


                }
            }
        });


        //for employee binding
        $.ajax({
            type: "GET",
            url: '/Enquiry/GetMasterDataList/',
            data: {
                'MasterName': 'Employee', 'term': ""
            },
            success: function (response) {
                debugger;
                if (response.Status == "ok") {
                    console.log(response.data);
                    var data = response.data;
                    var selectedjobid = [];
                    if ($('#AssignedEmployees').val() != '') {
                        selectedjobid = $('#AssignedEmployees').val().split(',');
                    }
                    $.each(data, function (index, item) {
                        var value = data[index].ID;
                        var label = data[index].Text;


                        var selected = false;
                        $.each(selectedjobid, function (val1, item1) {
                            if (value == item1)
                                selected = true;
                        });
                        if (selected == true) {
                            var html = '<option selected value="' + value + '">' + label + '</option>';
                            $('#drpAssignedEmployees').append(html);
                        }
                        else {
                            var html = '<option value="' + value + '">' + label + '</option>';
                            $('#drpAssignedEmployees').append(html);
                        }
                    })
                    /*$('#drpAssignedEmployees').select2();*/

                    $('#drpAssignedEmployees').select2({
                        width: '100%',
                        dropdownParent: $('#engineerpopup')   // adjust to your modal's ID
                    });

                } else {


                }
            }
        });

        //for client binding
        $.ajax({
            type: "GET",
            url: '/Enquiry/GetClientName',
            data: {
                'term': '', 'ClientType': ""
            },
            success: function (data) {
                debugger;


                $.each(data, function (index, item) {
                    console.log(data);
                    var value = data[index].ClientID;
                    var label = data[index].ClientName;


                    var selected = false;
                    //$.each(selectedjobid, function (val1, item1) {
                    //    if (value == item1)
                    //        selected = true;
                    //});
                    if (selected == true) {
                        var html = '<option selected value="' + value + '">' + label + '</option>';
                        $('#drpClientID').append(html);
                    }
                    else {
                        var html = '<option value="' + value + '">' + label + '</option>';
                        $('#drpClientID').append(html);
                    }
                })

                $('#drpClientID').select2({
                    width: '100%',
                   /* dropdownParent: $('#customerpopup')   // adjust to your modal's ID*/
                });


            }
        });
                
        // Open Select2 dropdown when any of these receives focus
        $('#EnquiryStageID, #drpEntityTypeIDs, #drpAssignedEmployees').on('focus', function () {
            const $this = $(this);
            setTimeout(() => $this.select2('open'), 0);
        });
        //$('#ProjectDescription').blur(function () {
        //    $('#drpAssignedEmployees').focus();
        //})
        //$('#drpAssignedEmployees').blur(function () {
        //    $('#drpEntityTypeIDs').focus();
        //})
        $('#EnquiryDueDate').blur(function () {
            $('#EnquiryStageID').focus();
            //$('#EnquiryStageID').select2('open');
        })
        $('#EnquiryStageID, #drpEntityTypeIDs, #drpAssignedEmployees').select2({
            tags: true,
            selectOnClose: true
        });
         
       
        //$('#EnquiryStageID').blur(function () {
        //    //$('#EnquiryStageID').focus();
        //  $('#drpAssignedEmployees').select2('open');
        //})
        
        //$('#CountryName').blur(function () {
        //    $('#PriorityID').focus();
            
        //})

        $('#drpClientID').blur(function () {
            $('#PriorityID').focus();

        })
        $('#EnquiryStatusID').blur(function () {
            $('#drpClientType').focus();
        })
        //$('#drpClientType').blur(function () {
        //    $('#drpClientID').select2('open');
        //})
      
    })

})(jQuery)