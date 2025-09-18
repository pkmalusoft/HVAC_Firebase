var supplierdetails;
function getsupplierdetail() {

    $.ajax({
        url: '/ProductMaster/GetSupplierName',
        datatype: "json",
        data: {
            term: $('#ContractorName').val()
        },
        success: function (data) {
            debugger;
            $('#ContractorID').val(0);
            if (data.length > 0) {

                $.each(data, function (index, val) {
                    debugger;
                    if (val.CustomerName == $('#ContractorName').val()) {
                        $("#ContractorName").val(val.SupplierName);

                        $('#ContractorID').val(val.SupplierID);
                        $('#ContractorcontactNo').val(val.mobileno);
                        $('#ContractorEmail').val(val.Email);
                        $('#ContractorAddress').val(val.Address1);
                        $('#sname').val(val.SupplierName);
                        $('#Contact').val(val.Contactperson);


                    }

                })
            }
        }
    })
}

$(document).ready(function () {
    // $('#ClientName').change(function () {

    //init();
    $("#ContractorName").autocomplete({

        source: function (request, response) {
            supplierdetails = null;
                $.ajax({
                url: '/ProductMaster/GetSupplierName',
                datatype: "json",
                data: {
                    term: request.term
                },
                success: function (data) {
                    response($.map(data, function (val, item) {
                        return {
                            label: val.SupplierName,
                            value: val.SupplierName,
                            SupplierID: val.SupplierID,
                            Phone: val.Phone,
                            Email: val.Email,
                            Address1: val.Address1,
                            Contactperson: val.Contactperson,
                            sname: val.SupplierName,

                        }
                    }))
                }
            })
        },
        minLength: 1,
        autoFocus: false,
        focus: function (e, ui) {
            e.preventDefault();
           

        },
        select: function (e, ui) {
            e.preventDefault();
            $("#ContractorName").val(ui.item.label);
            supplierdetails = ui.item;
            console.log(supplierdetails);
           
        },

    });

    $("#btnadd").click(function () {
        //if ($('#PaymentModeId').val() == '5') {
        getsupplierdetail();
        //}

    })
    var supplierids = [];

    $('#btnsupadd').click(function () {
        debugger;
        if (supplierdetails == null) {

            alert("please select sup");
            return false;
        }

        else {
            supplierids.push(supplierdetails.SupplierID);

            var RowCount = $('#SupBody > tr').length;

            var RowHtml = '<tr id="tr_' + RowCount + '">'
            //RowHtml = RowHtml + producttypehtml;

            RowHtml = RowHtml + '<td> <label for="ContractorName_" class="form-label">' + supplierdetails.sname + '</label> </td > '

            RowHtml = RowHtml + '<td>   <label for="ContactPerson_" class="form-label">' + supplierdetails.Contactperson + '</label></td>'


            RowHtml = RowHtml + '<td>   <label for="Phone_" class="form-label">' + supplierdetails.Phone + '</label></td>'


            RowHtml = RowHtml + '<td>   <label for="Email_" class="form-label">' + supplierdetails.Email + '</label></td>'

            RowHtml = RowHtml + '<td>   <label for="location_" class="form-label">' + supplierdetails.Address1 + '</label></td>'

            RowHtml = RowHtml + '</tr>';
            $('#SupBody').append(RowHtml);
        }

    });
    var Items = [];
    function SaveProducts() {
        debugger;
        var id = 0;
        var productPriceid = 0;
        if ($('#ProductID').val() > 0) {
            id = $('#ProductID').val();

        }
        var protypeobj = {
            ProductID: id,
            ProductName: $('#ProductName').val(),
            ProductTypeID: $('#ProductTypeID').val(),
            ProductGroupID: $('#ProductGroupID').val(),
            ProductCategoryID: $('#ProductCategoryID').val(),
            StockKeepingUnit: $('#StockKeepingUnit').val(),
            barcode: $('#barcode').val(),
            BrandID: $('#BrandID').val(),
            IsActive: $('#IsActive').val(),
            CreatedBy: $('#CreatedBy').val(),
            Createdat: $('#Createdat').val(),
            Price: $('#Price').val(),
            //Price: $('#Price').val(),

        }
        Items.push(protypeobj);
        
        debugger;
        $.ajax({
            type: "POST",
            url: '/ProductMaster/SaveProduct/',
            datatype: "html",
            data: { ProType: protypeobj, supplierid: supplierids, Details: JSON.stringify(Items) },
            success: function (response) {
                if (response.status == "OK") {
                    Swal.fire("Save Status!", response.message, "success");
                    setTimeout(function () {
                        window.location.href = "/ProductMaster/Index";
                    }, 100)


                }
                else {
                    Swal.fire("Save Status!", response.message, "warning");
                }


            }
        });





    }
  

    $("#btnsave").click(function () {
        debugger;
        SaveProducts();

    });
 

    $('#AddItem').click(function () {
        debugger;
        var productTypeID = $("#PriceTypeID").val();
        var productName = $("#ProductID").val();
        var TaxType = $("#TaxType").val();
        var Price = $('#Price').val();
        var Taxperc = $('#Taxperc').val();
        var EffectiveDate = $('#EffectiveDate').val();
        var taxhtml = "";
        var _taxdetail = "";
       
        


        var RowCount = $('#PriceBody > tr').length;

        var RowHtml = '<tr id="tr_' + RowCount + '">'
        //RowHtml = RowHtml + producttypehtml;

        RowHtml = RowHtml + '<td><input type="text" class="form-select"   id="PriceTypeID_' + RowCount + '" value="' + productName + '" /></td>';
        RowHtml = RowHtml + '<td><input type = "text" class="form-control" id = "Price_' + RowCount + '"  value = "' + Price + '" /></td>';

        RowHtml = RowHtml + '<td><input type="checkbox" id="Price_" value="' + Price + '"  /></td>'

        RowHtml = RowHtml + '<td><input type="text" class="form-control"   id="TaxType__' + RowCount + '" value="' + TaxType + '" /></td>';
        RowHtml = RowHtml + '<td><input type = "text" class="form-control" id = "Taxperc_' + RowCount + '"  value = "' + Taxperc + '" /></td>'
        RowHtml = RowHtml + '<td><div class="input-group" ><input type="text" class="form-control docs-date hasDatepicker" name="FromDate" id="FromDate" value="12-02-2025" placeholder="Pick a From Date" autocomplete="off"><button type="button" class="btn btn-secondary docs-datepicker-trigger" disabled=""> <i class="mdi mdi-calendar" aria-hidden="true"></i></button></div></td > ';

        RowHtml = RowHtml + '</tr>';
        $('#PriceBody').append(RowHtml);
        $('#ProductID_' + RowCount).val($('#ProductID').val()).trigger('change');

        $("#ProductName_" + RowCount).autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/Product/GetProductName',
                    datatype: "json",
                    data: {
                        term: request.term, ProductTypeID: $('#ProductID_' + RowCount).val()
                    },
                    success: function (data) {
                        response($.map(data, function (val, item) {
                            return {
                                label: val.ProductName,
                                value: val.ProductID,
                            }
                        }))
                    }
                })
            }, minLength: 0,
            autoFocus: false,
            focus: function (e, i) {
                e.preventDefault();
                $("#ProductName_" + RowCount).val(i.item.label);
                $('#ProductID_' + RowCount).val(i.item.value);
                $('#Price_' + RowCount).val(i.item.Price);

            },
            select: function (e, i) {
                e.preventDefault();
                $("#ProductName_" + RowCount).val(i.item.label);
                $('#ProductID_' + RowCount).val(i.item.value);
                $('#Price_' + RowCount).val(i.item.Price);
            }
        });
        $('#Price').val('');
        $('#TaxType').val('');
        $('#Taxperc').val('');
        $('#ProductID').val('');

    });

    //})
});