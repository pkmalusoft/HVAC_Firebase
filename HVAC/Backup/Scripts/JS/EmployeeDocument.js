 
function Downloadfile(filename) {
    $.ajax({
        url: '/Home/DownloadFile1',
        type: "GET",
        data: { objName: filename },
        success: function (result) {
            console.log(result);
            if (result == "OK") {
                window.location.href = "/Home/DownloadFile?filename=" + filename;
                //alert(result.FileName);
                //$('#Filename').val(result.FileName);
                //$.ajax({
                //    url: '/Consignment/DownloadFile',
                //    type: "GET",
                //    data: { objName: filename },
                //    success: function (result) {
                //        console.log(result);                            
                //    },
                //    error: function (err) {
                //        alert(err.statusText);
                //    }
                //});
            }
        },
        error: function (err) {
            alert(err.statusText);
        }
    });

}
function clearDocument() {
    $('#DocumentID').val('');
    $('#DocumentTitle').val('');
    $('#DocumentTypeID').val('');    
    $('#Filename').val('');
    $('#FileUpload1').val('');
    //$('#CNPieces').val('');
    //$('#CNWeight').val('');
    $('#btnSaveDocuments').html('Add & Save');
    var idtext = 'doctr_'
    $('[id^=' + idtext + ']').each(function (index, item) {
        $('#doctr_' + index).removeClass('rowactive');
    });
}

function setdocumentrowactive(index1) {
    var idtext = 'doctr_'
    $('[id^=' + idtext + ']').each(function (index, item) {
        $('#doctr_' + index).removeClass('rowactive');
    });
    $('#doctr_' + index1).addClass('rowactive');
}
function ShowDocument() {
    clearDocument();
    $.ajax({
        type: 'POST',
        url: '@Url.Action("ShowDocumentEntry", "JOBGenerate")',
        datatype: "html",
        data: {
            ConsignmentID: 0, JOBID: $('#JobID').val()
        },
        success: function (data) {
            $("#DocumentEntryContainer").html(data);
            //$('#Documentpopup').modal('show');
        }
    });

}
function SaveDocument() {
    debugger;
    var fileUpload = $("#FileUpload1").get(0);

    if ($('#DocumentTypeID').val() == '') {
        Swal.fire("Data Validation!", 'Select  Document Type!', "warning");
        
        $('#DocumentTypeID').focus();
        return;
    }
    else if ($('#DocumentTitle').val() == '' || $('#DocumentTitle').val() == '0') {
        
        Swal.fire("Data Validation!", 'Enter Document Title!', "warning");
        $('#DocumentTitle').focus();
        return;
    }
    else if (fileUpload.files.length == 0 && ($('#Filename').val()==null || $('#Filename').val()=='') ) {
        
        Swal.fire("Data Validation!",'Upload File', "warning");
        $('#FileUpload1').focus();
        return;
    }
    $('#btnSaveDocuments').attr('disabled', 'disabled');
    debugger;
    
    

    $.ajax({
        type: "Post",
        url: '/EmployeeMaster/SaveDocument/',        
        data: {
            DocumentID: $('#DocumentID').val(),
            DocumentTitle: $('#DocumentTitle').val(),
            DocumentTypeID: $('#DocumentTypeID').val(),
            //AcJournalID: $('#AcJournalID').val(),
            //CompanyID: $('#CompanyID').val(),
            //BranchID: $('#BranchID').val(),
            EmployeeID: $('#EmployeeID').val(),
            Filename: $('#Filename').val() },
        //datatype: "Json",
        //contentType:'application/json',
        success: function (response) {
            debugger;
            if (response.status == "OK") {
                Swal.fire("Save Status!", response.message, "success");
                reloaddocument();

            }
            else {
                Swal.fire("Save Status!", response.message, "warning");
                reloaddocument();
            }
            

        }
    });
}
function reloaddocument() {


   
        $.ajax({
            type: "POST",
            url: '/EmployeeMaster/ListDocument',
            datatype: "html",
            data: { id: $('#EmployeeID').val() },
            success: function (data) {
                debugger;
                $("#DocumentContainer").html(data);

                $('#btnSaveDocuments').removeAttr('disabled');
                clearDocument();


            }
        });
   
}
function EditDocument(id, index1) {
    $.ajax({
        type: "POST",
        url: '/EmployeeMaster/EditDocument/',
        datatype: "html",
        data: { id: id },
        success: function (response) {
            clearDocument();
            console.log(response.data);
            var data = response.data;
            $('#DocumentID').val(data.DocumentID);
            $('#DocumentTitle').val(data.DocumentTitle);
            $('#DocumentTypeID').val(data.DocumentTypeID).trigger('change');
            $('#Filename').val(data.Filename);
            //$("#DocumentEntryContainer").html(data);
            $('#btnSaveDocuments').html('Update');
            setdocumentrowactive(index1);
        }
    });
}

function DeleteDocument(DocumentId, index1) {
    setdocumentrowactive(index1);
    var ID = 0;
     
        ID = $('#EmployeeID').val();
    

    Swal.fire({ title: "Are you sure?", text: "You won't be able to revert this!", icon: "warning", showCancelButton: !0, confirmButtonColor: "#34c38f", cancelButtonColor: "#f46a6a", confirmButtonText: "Yes, delete it!" }).then(
        function (t) {
            if (t.value) {
                $.ajax({
                    type: "POST",
                    url: '/EmployeeMaster/DeleteDocument',
                    datatype: "html",
                    data: {
                        'DocumentId': DocumentId, 'ID': ID
                    },
                    success: function (data) {
                        // alert(data);
                        $("#DocumentContainer").html(data);
                        reloaddocument();
                        Swal.fire("Deleted!", "Your file has been deleted.", "success");

                    }
                });
            }
        });
    
     
}

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

                            SaveDocument();

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

       
        $('#imglogo').click(function () {

            $("#FileUpload1").trigger('click');

        });

        $("#FileUpload1").change(function () {
            var fileUpload = $("#FileUpload1").get(0);
            var files = fileUpload.files;
            if (files.length > 0) {
                $('#btnUpload').trigger('click');
            }
        });

        $('#btnUpload').click(function () {
           
            // Checking whether FormData is available in browser
            if (window.FormData !== undefined) {
                $('#lbfileuploadstatus').html('File Uploading..');
                $('#btnSaveDocuments').attr('disabled', 'disabled');
                var fileUpload = $("#FileUpload1").get(0);
                var files = fileUpload.files;

                // Create FormData object
                var fileData = new FormData();

                // Looping over all files and add it to FormData object
                for (var i = 0; i < files.length; i++) {
                    fileData.append(files[i].name, files[i]);
                }

                // Adding one more key to FormData object
                fileData.append('username', "Manas");

                $.ajax({
                    url: '/Home/UploadFiles',
                    type: "POST",
                    contentType: false, // Not to set any content header
                    processData: false, // Not to process data
                    data: fileData,
                    success: function (result) {
                        if (result.status == "ok") {
                            $('#imglogo').attr('src', "/UploadDocuments/" + result.FileName);
                            //alert(result.FileName);
                            $('#Filename').val(result.FileName);
                            $('#btnSaveDocuments').removeAttr('disabled');
                            $('#lbfileuploadstatus').html('File Uploaded Successfully,Click Save Now!',)
                            //Swal.fire("File Uploading Status!", "File Uploaded Successfully", "success");

                        }
                    },
                    error: function (err) {
                        alert(err.statusText);
                        $('#lbfileuploadstatus').html(err.statusText);
                        $('#btnSaveDocuments').removeAttr('disabled');
                    }
                });
            } else {
                alert("FormData is not supported.");
                $('#lbfileuploadstatus').html('');
                $('#btnSaveDocuments').removeAttr('disabled');
            }
            
        });

      
    });
