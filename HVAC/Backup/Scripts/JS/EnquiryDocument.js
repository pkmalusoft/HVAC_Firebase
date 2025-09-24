 
function Downloadfile(filename) {
    $.ajax({
        url: '/Eqnuiry/DownloadFile1',
        type: "GET",
        data: { objName: filename },
        success: function (result) {
            console.log(result);
            if (result == "OK") {
                window.location.href = "/Enquiry/DownloadFile?filename=" + filename;
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
 

function SaveDocument() {
    debugger;
  
    var fileUpload = $("#FileUpload1").get(0);

    if ($('#DocumentTypeID').val() == '') {
       
        $('#spandocerror').html('Select Document Type!');
        $('#DocumentTypeID').focus();
        return;
    }
    else if ($('#DocumentTitle').val() == '' || $('#DocumentTitle').val() == '0') {
        
        $('#spandocerror').html('Enter Document Title!');
        $('#DocumentTitle').focus();
        return;
    }
    
    else if ($('#DocumentLink').val() == "" && (fileUpload.files.length == 0 && ($('#Filename').val()==null || $('#Filename').val()=='') )) {
      
        $('#spandocerror').html('Upload Files or Update Document Link!');
        $('#FileUpload1').focus();
        return;
    }
    $('#btnSaveDocuments').attr('disabled', 'disabled');
     

    $.ajax({
        type: "POST",
        url: '/Enquiry/SaveDocument/',
        datatype: "json",
        data: {
            DocumentID: $('#DocumentID').val(),
            EnquiryID: $('#EnquiryID').val(),
            DocumentTitle: $('#DocumentTitle').val(),
            DocumentTypeID: $('#DocumentTypeID').val(),
            DocumentLink: $('#DocumentLink').val(),
            Filename: $('#Filename').val() },
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
                $('#documentpopup').modal('hide');
                reloaddocument();

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
                reloaddocument();
            }
            

        }
    });
}
function reloaddocument() {
    $.ajax({
        type: "POST",
        url: '/Enquiry/ListDocument',
        datatype: "html",
        data: { id: $('#EnquiryID').val() },
        success: function (data) {
            debugger;
            $("#enquiry-document").html(data);
            
            $('#btnSaveDocuments').removeAttr('disabled');
            clearDocument();


        }
    });
}
function EditDocument(id, index1) {

    debugger;
    $.ajax({
        type: 'POST',
        url: '/Enquiry/ShowDocumentEntry',
        datatype: "html",
        data: {
            id: id, EnquiryID: $('#EnquiryID').val()
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
    //$.ajax({
    //    type: "POST",
    //    url: '/Enquiry/ShowDocumentEntry/',
    //    datatype: "html",
    //    data: { id: id,EnquiryID:$('#EnquriyID').val() },
    //    success: function (response) {
    //        clearDocument();
    //        console.log(response.data);
    //        var data = response.data;
    //        $('#DocumentID').val(data.DocumentID);
    //        $('#DocumentTitle').val(data.DocumentTitle);
    //        $('#DocumentTypeID').val(data.DocumentTypeID).trigger('change');
    //        $('#Filename').val(data.Filename);
    //        //$("#DocumentEntryContainer").html(data);
    //        $('#btnSaveDocuments').html('Update');
    //        $('#documentpopup').modal(show);
    //        setdocumentrowactive(index1);
    //    }
    //});
}

function DeleteDocument(DocumentId, index1) {
    setdocumentrowactive(index1);
    Swal.fire({ title: "Are you sure?", text: "You won't be able to revert this!", icon: "warning", showCancelButton: !0, confirmButtonColor: "#34c38f", cancelButtonColor: "#f46a6a", confirmButtonText: "Yes, delete it!" }).then(
        function (t) {
            if (t.value) {
                $.ajax({
                    type: "POST",
                    url: '/Enquiry/DeleteDocument',
                    datatype: "html",
                    data: {
                        'DocumentId': DocumentId, 'EnquiryID': $('#EnquiryID').val()
                    },
                    success: function (data) {
                        // alert(data);
                        $("#enquiry-document").html(data);

                        //reloaddocument();
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
            var t = document.getElementsByClassName("documentneeds-validation");
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

        
        
        //$('#imglogo').click(function () {

        //    $("#FileUpload1").trigger('click');

        //});

      

      
    });
