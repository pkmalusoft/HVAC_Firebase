

function SaveProject() {
    debugger;
    $('#btnSave').attr('disabled', 'disabled');
    var itemcount = $('#ItemBody >  tr').length;
     var itemcount = $('#ItemBody > tr').length;
    var idtext = 'txtCityName_';
    var Projobj = {
        ProjectName: $('#ProjectName').val(),
        ProjectDescription: $('#ProjectDescription').val(),
        ProjectType: $('#ddTypes').val(),
        ProjectCode: $('#ProjectCode').val(),
        ProjectID: $('#ProjectID').val(),
        ClientID: $('#CustomerID').val(),
        CustomerID: $('#CustomerID').val(),
        ProjectManager: $('#ddEmployees').val(),
        StartDate: $('#StartDate').val(),
        EndDate: $('#EndDate').val(),
        Priority: $('#Priority').val(),
        Status: $('#ddStatus').val(),
        Budget: $('#Budget').val(),
        CurrencyID: $('#ddCurrency').val(),
        ActualCost: $('#ActualCost').val(),
        Duration: $('#Duration').val(),      


    };

    var itemcount = $('#ItemBody > tr').length;
    if (itemcount > 0) {
        SavePromileStone(Projobj);
    }   
    else {
        $.ajax({
            type: "POST",
            url: '/ProjectMaster/SaveProject/',
            dataType: "json",
            data: { project: Projobj, Details: "" },
            success: function (response) {
                debugger;

                if (response.status === "OK") {
                    Swal.fire("Save Status!", response.message, "success").then(function () {
                        $('#ProjectID').val(response.ProjectID);
                        window.location.href = '/ProjectMaster/Index/0';
                    });

                } else {

                    $('#btnSave').removeAttr('disabled');
                    Swal.fire("Save Status!", response.message, "warning");
                }
            },
            error: function (xhr, status, error) {

                $('#btnSave').removeAttr('disabled');
                Swal.fire("Error", "There was an error saving the project. Please try again.", "error");
            }
        });

    }
}
   
function SavePromileStone(Projobj) {
    var itemcount = $('#ItemBody > tr').length;
    var idtext = 'txtmilestonegroup_';
    var Items = [];
    if (itemcount > 0) {
        $('[id^=' + idtext + ']').each(function (index, item) {
            var deleted = $('#hdndeleted_' + index).val();
            var _deleted = false;
            if (deleted == "true")
                _deleted = true;



            var data = {
                Milestonegroup: $('#txtmilestonegroup_' + index).val(),
                Description: $('#txtdescription_' + index).val(),
                Date: $('#txtestimatedate_' + index).val(),
                EmployeeId: $('#hdnempid_' + index).val(),
                MilestoneGroupID: $('#hdnmilestoneid_' + index).val(),
                Deleted: _deleted
            }
            Items.push(data);


            if ((index + 1) == itemcount) {
                var ItemDetails = JSON.stringify(Items);
                $.ajax({
                    type: "POST",
                    url: '/ProjectMaster/SaveProject/',
                    dataType: "json",
                    data: { project: Projobj, Details: ItemDetails },
                    success: function (response) {
                        debugger;

                        if (response.status === "OK") {
                            Swal.fire("Save Status!", response.message, "success").then(function () {
                                $('#ProjectID').val(response.ProjectID);
                                window.location.href = '/ProjectMaster/Index/0';
                            });

                        } else {

                            $('#btnSave').removeAttr('disabled');
                            Swal.fire("Save Status!", response.message, "warning");
                        }
                    },
                    error: function (xhr, status, error) {

                        $('#btnSave').removeAttr('disabled');
                        Swal.fire("Error", "There was an error saving the project. Please try again.", "error");
                    }
                });
            }
        });
    }
    else {
        return '';
    }
}

function initformControl() {

    $.ajax({
        type: "POST",
        url: "/ProjectMaster/GetProjectMilStone",
        datatype: "Json",
        data: { id: $("#ProjectID").val() },
        success: function (data) {
            if ($('#ItemBody >  tr').length == 1) {
                var emptyrow = $('#ItemBody > tr').html();
                if (emptyrow.indexOf('No data available in table') >= 0) {
                    $('#ItemBody').html('');
                }
            }
            var itemCount = 0;
            $.each(data, function (index, value) {
                itemCount = index;


                var newRowHtml = '<tr id="tr_' + itemCount + '">' +
                    '<td><input type="text" id="txtmilestonegroup_' + itemCount + '" value="' + value.Milestonegroup + '" class="form-select" disabled />' +
                    '<input type="hidden" id="hdnmilestoneid_' + itemCount + '" value="' + value.MilestoneGroupID + '" /></td>' +
                    '<td><input type="text" id="txtdescription_' + itemCount + '" value="' + value.Description + '" disabled class="form-control" /></td>' +
                    '<td><input type="text" id="txtestimatedate_' + itemCount + '" value="' + value.Date + '" class="form-control" /></td>' +
                    '<td><input type="text" id="txtemployee_' + itemCount + '" value="' + value.EmployeeName + '" disabled class="form-control" />' +
                    '<input type="hidden" id="hdnempid_' + itemCount + '" value="' + value.EmployeeID + '" /></td>' +
                    '<td style="text-align:left;"><a href="javascript:void(0)" class="text-danger" onclick="deleteItemtrans(this)"><i class="mdi mdi-delete font-size-18"></i></a></td>' +
                    '</tr>';

                $("#ItemBody").append(newRowHtml);










            });

        }
    });
}
function init() {
    initformControl();
}
window.addEventListener(
    "load",
    function () {
        init();
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

                        SaveProject();

                    }
                },
                !1
            );
        });
    },
    !1
);