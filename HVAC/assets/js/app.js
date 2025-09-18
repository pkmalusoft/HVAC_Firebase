/*
Template Name: Skote - Admin & Dashboard Template
Author: Themesbrand
Version: 3.2.0
Website: https://themesbrand.com/
Contact: themesbrand@gmail.com
File: Main Js File
*/
function CheckPeriodLock(vEntryDate) {
    $.ajax({
        type: "Get",
        url: "/PeriodLocks/CheckPeriodLock",
        data: { vEntryDate: vEntryDate },
        async :false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            console.log(response);
            if (response.PeriodLock == true) {
                Swal.fire("Data Validation!", response.message, "warning");
                return true;
            }
            else {
                return false;
            }
        }
    })
}
function ShowPostingJournal(id) {

    $.ajax({
        type: "POST",
        url: '/Accounts/ShowPosting',
        datatype: "html",
        data: {
            'id': id
        },
        success: function (data) {
            $("#PostingJournalContainer").html(data);
            $.ajax({
                type: "GET",
                url: "/Accounts/GetJournalAcJDetails",
                datatype: "Json",
                data: { id: id },
                success: function (data) {


                    $.each(data, function (index, value) {

                        var i = $('#postingdetails > tr').length;
                        var ID = value.ID;
                        var Achead = value.AcHead;
                        var acheadid = value.acHeadID;
                        var CreditAmount = value.CreditAmount;
                        var DebitAmount = value.DebitAmount;
                        var remarks = value.AcRemark;
                        var html = '<tr>' + '<td><input type="hidden" id="AcJournalDetID_' + i + '" value="' + ID + '"><input id="hdndeleted_' + i + '" type ="hidden" name ="acJournalDetailsList[' + i + '].IsDeleted" class="hdndeleted" value="false"   /> <input id="AcHead_' + i + '" type="hidden" readonly class="form-control" value="' + Achead + '" name="acJournalDetailsList[' + i + '].AcHead"><input type="hidden" id="AcHeadId_' + i + '" value=' + acheadid + ' name="acJournalDetailsList[' + i + '].acHeadID">' + Achead + '</td>';
                        html += '<td class="textright"><input  readonly class="textright form-control" type="hidden"  onchange="CalculateTotal(' + i + ',1)" value=' + parseFloat(DebitAmount).toFixed(2) + ' name="acJournalDetailsList[' + i + '].DebitAmount" id="JDebitAmount_' + i + '" />' + numberWithCommas(DebitAmount) + '</td>';
                        html += '<td class="textright"><input readonly class="textright form-control" type="hidden"  onchange="CalculateTotal(' + i + ',2)" value=' + parseFloat(CreditAmount).toFixed(2) + ' name="acJournalDetailsList[' + i + '].CreditAmount" id="JCreditAmount_' + i + '" />'+  numberWithCommas(CreditAmount) + '</td>';
                        //html +='<td>' + PaymentTypetext + '<input type="hidden" id="Paytype_' + i + '" value=' + PaymentType + ' name="acJournalDetailsList[' + i + '].IsDebit"></td>' +
                        html += '<td><input readonly type="hidden" id="txtremark_' + i + '" class="form-control" value="' + remarks + '" name="acJournalDetailsList[' + i + '].AcRemark"/><span style="text-wrap:wrap;min-width:150px">' + remarks + '</span></td>';
                        html += '</tr >';

                        $('#postingdetails').append(html);
                        if ((index + 1) == data.length) {
                            CalculateTotal();

                        }
                    });
                }


            });
            $('#postingjournalpopup').modal('show');
        }
    });
}
(function ($) {

    'use strict';

    var language = localStorage.getItem('language');
    // Default Language
    var default_lang = 'en';

    function setLanguage(lang) {
        if (document.getElementById("header-lang-img")) {
            if (lang == 'en') {
                document.getElementById("header-lang-img").src = "/assets/images/flags/us.jpg";
            } else if (lang == 'sp') {
                document.getElementById("header-lang-img").src = "/assets/images/flags/spain.jpg";
            }
            else if (lang == 'gr') {
                document.getElementById("header-lang-img").src = "/assets/images/flags/germany.jpg";
            }
            else if (lang == 'it') {
                document.getElementById("header-lang-img").src = "/assets/images/flags/italy.jpg";
            }
            else if (lang == 'ru') {
                document.getElementById("header-lang-img").src = "/assets/images/flags/russia.jpg";
            }
            localStorage.setItem('language', lang);
            language = localStorage.getItem('language');
            getLanguage();
        }
    }

    // Multi language setting
    function getLanguage() {
        (language == null) ? setLanguage(default_lang) : false;
        $.getJSON('/assets/lang/' + language + '.json', function (lang) {
            $('html').attr('lang', language);
            $.each(lang, function (index, val) {
                (index === 'head') ? $(document).attr("title", val['title']) : false;
                $("[key='" + index + "']").text(val);
            });
        });
    }

    //millions format
    function addCommas(nStr) {

        nStr += '';
        var x = nStr.split('.');
        var x1 = x[0];
        var x2 = x.length > 1 ? '.' + x[1] : '';
        var rgx = /(\d+)(\d{3})/;
        while (rgx.test(x1)) {
            x1 = x1.replace(rgx, '$1' + ',' + '$2');
        }
        return x1 + x2;
    }

    function getnumberformatLakhs(nStr) {
        var x = nStr.split('.');
        var x1 = x[0];
        var x2 = x.length > 1 ? '.' + x[1] : '';
        var lastThree = x1.substring(x1.length - 3);
        var otherNumbers = x1.substring(0, x1.length - 3);
        if (otherNumbers != '')
            lastThree = ',' + lastThree;
        var res = otherNumbers.replace(/\B(?=(\d{2})+(?!\d))/g, ",") + lastThree + x2;
        return res;
    }

    function RestrictSpaceSpecial(e) {
        var k;
        document.all ? k = e.keyCode : k = e.which;
        //console.log(e.keyCode);
        if ((k >= 48 && k <= 57) || k == 46)
            return true;
        else
            return false;
        //return ((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57));
    }
    function RestrictSpaceSpecialamount(e) {
        var k;
        document.all ? k = e.keyCode : k = e.which;
        //console.log(e.keyCode);
        if ((k >= 45 && k <= 57) || k == 46 || k == 44)
            return true;
        else
            return false;
        //return ((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57));
    }
    function initMetisMenu() {
        //metis menu        
        $("#side-menu").metisMenu();
    }

    function initLeftMenuCollapse() {
        $('#vertical-menu-btn').on('click', function (event) {
            event.preventDefault();
            $('body').toggleClass('sidebar-enable');
            if ($(window).width() >= 992) {
                $('body').toggleClass('vertical-collpsed');
            } else {
                $('body').removeClass('vertical-collpsed');
            }
        });
    }

    function initActiveMenu() {
        // === following js will activate the menu in left side bar based on url ====
        $("#sidebar-menu a").each(function () {
            var pageUrl = window.location.href.split(/[?#]/)[0];
            if (this.href == pageUrl && this.href != '#' && this.href != '') {
                $(this).addClass("active");
                $(this).parent().addClass("mm-active"); // add active to li of the current link
                $(this).parent().parent().addClass("mm-show");
                $(this).parent().parent().prev().addClass("mm-active"); // add active class to an anchor
                $(this).parent().parent().parent().addClass("mm-active");
                $(this).parent().parent().parent().parent().addClass("mm-show"); // add active to li of the current link
                $(this).parent().parent().parent().parent().parent().addClass("mm-active");
            }
        });
    }

    function initMenuItemScroll() {
        // focus active menu in left sidebar
        $(document).ready(function () {
            if ($("#sidebar-menu").length > 0 && $("#sidebar-menu .mm-active .active").length > 0) {
                var activeMenu = $("#sidebar-menu .mm-active .active").offset().top;
                if (activeMenu > 300) {
                    activeMenu = activeMenu - 300;
                    $(".vertical-menu .simplebar-content-wrapper").animate({ scrollTop: activeMenu }, "slow");
                }
            }
        });
    }

    function initHoriMenuActive() {
        $(".navbar-nav a").each(function () {
            var pageUrl = window.location.href.split(/[?#]/)[0];
            if (this.href == pageUrl) {
                $(this).addClass("active");
                $(this).parent().addClass("active");
                $(this).parent().parent().addClass("active");
                $(this).parent().parent().parent().addClass("active");
                $(this).parent().parent().parent().parent().addClass("active");
                $(this).parent().parent().parent().parent().parent().addClass("active");
                $(this).parent().parent().parent().parent().parent().parent().addClass("active");
            }
        });
    }

    function initFullScreen() {
        $('[data-bs-toggle="fullscreen"]').on("click", function (e) {
            e.preventDefault();
            $('body').toggleClass('fullscreen-enable');
            if (!document.fullscreenElement && /* alternative standard method */ !document.mozFullScreenElement && !document.webkitFullscreenElement) {  // current working methods
                if (document.documentElement.requestFullscreen) {
                    document.documentElement.requestFullscreen();
                } else if (document.documentElement.mozRequestFullScreen) {
                    document.documentElement.mozRequestFullScreen();
                } else if (document.documentElement.webkitRequestFullscreen) {
                    document.documentElement.webkitRequestFullscreen(Element.ALLOW_KEYBOARD_INPUT);
                }
            } else {
                if (document.cancelFullScreen) {
                    document.cancelFullScreen();
                } else if (document.mozCancelFullScreen) {
                    document.mozCancelFullScreen();
                } else if (document.webkitCancelFullScreen) {
                    document.webkitCancelFullScreen();
                }
            }
        });
        document.addEventListener('fullscreenchange', exitHandler);
        document.addEventListener("webkitfullscreenchange", exitHandler);
        document.addEventListener("mozfullscreenchange", exitHandler);
        function exitHandler() {
            if (!document.webkitIsFullScreen && !document.mozFullScreen && !document.msFullscreenElement) {
                console.log('pressed');
                $('body').removeClass('fullscreen-enable');
            }
        }
    }

    function initRightSidebar() {
        // right side-bar toggle
        $('.right-bar-toggle').on('click', function (e) {
            $('body').toggleClass('right-bar-enabled');
        });

        $(document).on('click', 'body', function (e) {
            if ($(e.target).closest('.right-bar-toggle, .right-bar').length > 0) {
                return;
            }

            $('body').removeClass('right-bar-enabled');
            return;
        });
    }

    function initDropdownMenu() {
        if (document.getElementById("topnav-menu-content")) {
            var elements = document.getElementById("topnav-menu-content").getElementsByTagName("a");
            for (var i = 0, len = elements.length; i < len; i++) {
                elements[i].onclick = function (elem) {
                    if (elem.target.getAttribute("href") === "#") {
                        elem.target.parentElement.classList.toggle("active");
                        elem.target.nextElementSibling.classList.toggle("show");
                    }
                }
            }
            window.addEventListener("resize", updateMenu);
        }
    }

    function updateMenu() {
        var elements = document.getElementById("topnav-menu-content").getElementsByTagName("a");
        for (var i = 0, len = elements.length; i < len; i++) {
            if (elements[i].parentElement.getAttribute("class") === "nav-item dropdown active") {
                elements[i].parentElement.classList.remove("active");
                if (elements[i].nextElementSibling !== null) {
                    elements[i].nextElementSibling.classList.remove("show");
                }
            }
        }
    }

    function initComponents() {
        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
        var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl)
        });

        var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'))
        var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
            return new bootstrap.Popover(popoverTriggerEl)
        });

        var offcanvasElementList = [].slice.call(document.querySelectorAll('.offcanvas'))
        var offcanvasList = offcanvasElementList.map(function (offcanvasEl) {
            return new bootstrap.Offcanvas(offcanvasEl)
        })
    }

    function initPreloader() {
        $(window).on('load', function () {
            $('#status').fadeOut();
            $('#preloader').delay(350).fadeOut('slow');
        });
    }

    function initSettings() {
        if (window.sessionStorage) {
            var alreadyVisited = sessionStorage.getItem("is_visited");
            if (!alreadyVisited) {
                sessionStorage.setItem("is_visited", "light-mode-switch");
            } else {
                $(".right-bar input:checkbox").prop('checked', false);
                $("#" + alreadyVisited).prop('checked', true);
                updateThemeSetting(alreadyVisited);
            }
        }
        $("#light-mode-switch, #dark-mode-switch, #rtl-mode-switch, #dark-rtl-mode-switch").on("change", function (e) {
            updateThemeSetting(e.target.id);
        });

        // show password input value
        $("#password-addon").on('click', function () {
            if ($(this).siblings('input').length > 0) {
                $(this).siblings('input').attr('type') == "password" ? $(this).siblings('input').attr('type', 'input') : $(this).siblings('input').attr('type', 'password');
            }
        })
    }

    function updateThemeSetting(id) {
        if ($("#light-mode-switch").prop("checked") == true && id === "light-mode-switch") {
            $("html").removeAttr("dir");
            $("#dark-mode-switch").prop("checked", false);
            $("#rtl-mode-switch").prop("checked", false);
            $("#dark-rtl-mode-switch").prop("checked", false);
            $("#bootstrap-style").attr('href', '/assets/css/bootstrap.min.css');
            $("#app-style").attr('href', '/assets/css/app.min.css');
            sessionStorage.setItem("is_visited", "light-mode-switch");
        } else if ($("#dark-mode-switch").prop("checked") == true && id === "dark-mode-switch") {
            $("html").removeAttr("dir");
            $("#light-mode-switch").prop("checked", false);
            $("#rtl-mode-switch").prop("checked", false);
            $("#dark-rtl-mode-switch").prop("checked", false);
            $("#bootstrap-style").attr('href', '/assets/css/bootstrap-dark.min.css');
            $("#app-style").attr('href', '/assets/css/app-dark.min.css');
            sessionStorage.setItem("is_visited", "dark-mode-switch");
        } else if ($("#rtl-mode-switch").prop("checked") == true && id === "rtl-mode-switch") {
            $("#light-mode-switch").prop("checked", false);
            $("#dark-mode-switch").prop("checked", false);
            $("#dark-rtl-mode-switch").prop("checked", false);
            $("#bootstrap-style").attr('href', '/assets/css/bootstrap-rtl.min.css');
            $("#app-style").attr('href', '/assets/css/app-rtl.min.css');
            $("html").attr("dir", 'rtl');
            sessionStorage.setItem("is_visited", "rtl-mode-switch");
        } else if ($("#dark-rtl-mode-switch").prop("checked") == true && id === "dark-rtl-mode-switch") {
            $("#light-mode-switch").prop("checked", false);
            $("#rtl-mode-switch").prop("checked", false);
            $("#dark-mode-switch").prop("checked", false);
            $("#bootstrap-style").attr('href', '/assets/css/bootstrap-dark-rtl.min.css');
            $("#app-style").attr('href', '/assets/css/app-dark-rtl.min.css');
            $("html").attr("dir", 'rtl');
            sessionStorage.setItem("is_visited", "dark-rtl-mode-switch");
        }

    }

    function initLanguage() {
        // Auto Loader
        if (language != null && language !== default_lang)
            setLanguage(language);
        $('.language').on('click', function (e) {
            setLanguage($(this).attr('data-lang'));
        });
    }

    function initCheckAll() {
        $('#checkAll').on('change', function () {
            $('.table-check .form-check-input').prop('checked', $(this).prop("checked"));
        });
        $('.table-check .form-check-input').change(function () {
            if ($('.table-check .form-check-input:checked').length == $('.table-check .form-check-input').length) {
                $('#checkAll').prop('checked', true);
            } else {
                $('#checkAll').prop('checked', false);
            }
        });
    }
    function getchildmenu(ParentID, response) {
        var html = '';
        for (var i = 0; i < response.length; i++) {
            var data = response[i];
          
            if (data.ParentID == ParentID && data.SubLevel>0) {

                if (html=='')
                    html = '<ul class="sub-menu" aria - expanded="true" >';
                debugger;
                console.log(data);
                
                var childhtml = '<li><a href="' + data.Link + '" key="t-light-sidebar">' + data.Title + '</a>'; 
                var childhtml1 = '';
                if (parseInt(data.SubLevel) > 0) {
                    childhtml1 = getchildmenu(data.MenuID, response);
                }
                if (childhtml1 == '' || childhtml1) {
                    childhtml = childhtml + '</li>';
                }
                else {
                    childhtml = childhtml + childhtml1 + '</li>';
                }
                html += childhtml;                
            }
            if ((i + 1) == response.length) {
                html += html + '</ul';
                return html;
            }

        }
        
    }
    function getchildmenu1(ParentID, response) {
        var html = '';
        for (var i = 0; i < response.length; i++) {
            var data = response[i];

            if (data.ParentID == ParentID && data.SubLevel > 0) {

                if (html == '')
                    html = '<ul class="sub-menu" aria - expanded="true" >';
                debugger;
                console.log(data);

                var childhtml = '<li><a href="' + data.Link + '" key="t-light-sidebar">' + data.Title + '</a>';
                var childhtml1 = '';
                if (parseInt(data.SubLevel) > 0) {
                    childhtml1 = getchildmenu(data.MenuID, response);
                }
                if (childhtml1 == '' || childhtml1) {
                    childhtml = childhtml + '</li>';
                }
                else {
                    childhtml = childhtml + childhtml1 + '</li>';
                }
                html += childhtml;
            }
            if ((i + 1) == response.length) {
                html += html + '</ul';
                return html;
            }

        }

    }

    function initMenuDisplay() {
        $.ajax({
            type: "GET",
            url: "/api/Data/GetMenuSidebar",
            data: { RoleId: 1},
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                console.log(response);
                for (var i = 0; i < response.length; i++)
                {
                    
                    var data = response[i];
                    if (data.ParentID == 0) {
                        debugger;
                        console.log(data);
                        var childhtml = '';
                        if (parseInt(data.SubLevel) > 0) {
                            childhtml = getchildmenu(data.MenuID, response); //'<li><a href=@Url.Action("LayoutsLightSidebar", "Layouts") key="t-light-sidebar">Light Sidebar</a></li>';
                        }
                        else {
                            childhtml = getchildmenu1(data.MenuID, response); //'<li><a href=@Url.Action("LayoutsLightSidebar", "Layouts") key="t-light-sidebar">Light Sidebar</a></li>';
                        }
                        var html = '<li><a href="javascript: void(0);" class="has-arrow waves-effect">'  +
                        '<i class="bx bx-layout"></i>' +
                            '<span key="t-layouts">' + data.Title + '</span ></a > ' + childhtml  + '</li>'                          
                        $('#side-menu').append(html);
                    }

                }

             
                
            }
        })
    }

    function init() {
        //initMenuDisplay(); //my own function
        initMetisMenu();
        initLeftMenuCollapse();
        initActiveMenu();
        initMenuItemScroll();
        initHoriMenuActive();
        initFullScreen();
        initRightSidebar();
        initDropdownMenu();
        initComponents();
        initSettings();
        initLanguage();
        initPreloader();
        Waves.init();
        initCheckAll();
        $('#vertical-menu-btn').trigger('click');
        var minDate = $('#hdnMinDate').val();
        var maxDate = $('#hdnMaxDate').val();
        var startdate = minDate;
        var enddate = maxDate;
        var sd = new Date(startdate.split('-')[1] + '-' + startdate.split('-')[0] + '-' + startdate.split('-')[2]);
        var ed = new Date(enddate.split('-')[1] + '-' + enddate.split('-')[0] + '-' + enddate.split('-')[2]);
        $('#FromDate').datepicker({
            dateFormat: 'dd-mm-yy',
            //minDate: sd,
            //maxDate: ed,
            onClose: function () {
                this.focus()
            }
            //onChangeMonthYear: function (dateText) {
            //    console.log("Selected date: " + dateText + "; input's current value: " + this.value);
            //},
            //onSelect: function (dateText) { //working
            //    console.log("Selected date: " + dateText + "; input's current value: " + this.value);
            //}
        });

        //$('.hasDatepicker').change(function () {
        //    $(this).val(getDate(this));
        //})
        //}).on('change', function (e) {
        //    $(this).val(getDate(this));
        //}),
        //var classtext = '.textrightamount';
        //$(".textrightamount").each(function () {
        //    $(this).html("Found this one");
        //});
        //$('[class^=' + classtext + ']').each(function (index, item) {

        //    alert('ss');
        //});
        $('#ToDate').datepicker({
            dateFormat: 'dd-mm-yy',
            //minDate: sd,
            //maxDate: ed,
            onClose: function () {
                this.focus()
            }
        }).on('changeDate', function (e) {
            $(this).datepicker('hide');
        });
        function getDate(element) {
            var date;
            try {
                date = $.datepicker.parseDate(dateFormat, element.value);
            } catch (error) {
                date = null;
            }

            return date;
        }

        // Date Picker


        $('#FromDate1').datepicker({
            dateFormat: 'dd-mm-yy',
            //minDate: sd,
            //maxDate: ed,
            onClose: function () {
                this.focus()
            }
        });
        $('#ToDate1').datepicker({
            dateFormat: 'dd-mm-yy',
            //minDate: sd,
            //maxDate: ed,
            onClose: function () {
                this.focus()
            }
        });
       
        $('#CollectedDate').datepicker({
            dateFormat: 'dd-mm-yy',
            minDate: sd,
            maxDate: ed
        });
        $('#CreatedDate').datepicker({
            dateFormat: 'dd-mm-yy',
            //minDate: sd,
            //maxDate: ed
        });
        $('#TransactionDate').datepicker({
            dateFormat: 'dd-mm-yy',
            //minDate: sd,
            //maxDate: ed
        });
        //$('#ManifestDate').datepicker({
        //    dateFormat: 'dd-mm-yy',
        //    minDate: sd,
        //    maxDate: ed
        //});
        $('#InvoiceDate').datepicker({
            dateFormat: 'dd-mm-yy',
            minDate: sd,
            maxDate: ed
        });
       
       
        $(".datetimepicker").each(function () {
            $(this).datetimepicker({
                format: 'd-m-y H:i',
                minDate: sd,
                maxDate: ed
               //formatDate :'dd-mm-yy H:i'
            });
        });
        //$('#DRSDate').datepicker({
        //    dateFormat: 'dd-mm-yy',
        //    minDate: sd,
        //    maxDate: ed
        //});
        //$('#ReceivedTime').datepicker({
        //    dateFormat: 'dd-mm-yy',
        //    minDate: sd,
        //    maxDate: ed
        //});
        //$('#DeliveredDate').datepicker({
        //    dateFormat: 'dd-mm-yy',
        //    minDate: sd,
        //    maxDate: ed
        //});
        $('#RecPayDate').datepicker({
            dateFormat: 'dd-mm-yy',
            minDate: sd,
            maxDate: ed,
            onClose: function () {
                this.focus()
            }
        });
        //$('#EntryDate').datepicker({
        //    dateFormat: 'dd-mm-yy',
        //    minDate: sd,
        //    maxDate: ed
        //});
        $('#PrintDate').datepicker({
            dateFormat: 'dd-mm-yy',
            minDate: sd,
            maxDate: ed,
            onClose: function () {
                this.focus()
            }
        });
        $('#transdate').datepicker({
            dateFormat: 'dd-mm-yy',
            minDate: sd,
            maxDate: ed,
            onClose: function () {
                this.focus()
            }
        }).on('changeDate', function (e) {
            debugger;
            console.log(CheckPeriodLock($(this).val()));
        });

        $('#TransDate').datepicker({
            dateFormat: 'dd-mm-yy',
            minDate: sd,
            maxDate: ed,
            onClose: function () {
                this.focus()
            }
        }) 
        $('#AsonDate').datepicker({
            dateFormat: 'dd-mm-yy',
            minDate: sd,
            maxDate: ed,
            onClose: function () {
                this.focus()
            }
        });
        
        $('#ReceivedDate').datepicker({
            dateFormat: 'dd-mm-yy',
            minDate: sd,
            maxDate: ed,
            onClose: function () {
                this.focus()
            }
        });
        
      
        $('#txtAWBDate').datepicker({
            dateFormat: 'dd-mm-yy',
            minDate: sd,
            maxDate: ed
        });
 
        $('#ReceiptDate').datepicker({
            dateFormat: 'dd-mm-yy',
            minDate: sd,
            maxDate: ed,
            onClose: function () {
                this.focus()
            }
        });
        $('#JoinDate').datepicker({
            dateFormat: 'dd-mm-yy',
            onClose: function () {
                this.focus()
            }
            //minDate: sd,
            //maxDate: ed
        });
        $('#RegExpirydate').datepicker({
            dateFormat: 'dd-mm-yy',
            minDate: sd,
            maxDate: ed
        });
        $('#PurchaseDate').datepicker({
            dateFormat: 'dd-mm-yy',
            minDate: sd,
            maxDate: ed,
            onClose: function () {
                this.focus()
            }
        });
        $('#ValueDate').datepicker({
            dateFormat: 'dd-mm-yy',
            minDate: sd,
            maxDate: ed,
            onClose: function () {
                this.focus()
            }
        });
        $('#Date').datepicker({
            dateFormat: 'dd-mm-yy',
            minDate: sd,
            maxDate: ed,
            onClose: function () {
                this.focus()
            }
        });
        $('#ChequeDate').datepicker({
            dateFormat: 'dd-mm-yy',
            //minDate: sd,
            //maxDate: ed,
            onClose: function () {
                this.focus()
            }
        });
        
        //$(".text-right").keypress(function (e) {
        //    return RestrictSpaceSpecial(e);
        //})

        //for numeric ,integer fields
        $(".textright").keypress(function (e) {
            return RestrictSpaceSpecial(e);
        })
        $(".textright3").keypress(function (e) {
            return RestrictSpaceSpecial(e);
        })
        $(".textright4").keypress(function (e) {
            return RestrictSpaceSpecial(e);
        })
        $(".textright").change(function () {
            debugger;
            var x = $(this);
            var str = $(this).val().trim();
            str = str.replaceAll(',', '');
            if (str != '' && str != null) {
                //$(this).val($(this).val().trim());
                //str = $(this).val();
                //console.log(str);
                var _decimal = $('#hdncompanydecimal').val();
                
                str = parseFloat(str).toFixed(_decimal);
                $(this).val(str);
                //$(this).val(parseFloat(str).toFixed(_decimal));
            }
            else {
                $(this).val(parseFloat(0).toFixed(_decimal));
            }

        });

        //for amount fields with format
        $(".textrightamount").keypress(function (e) {
            return RestrictSpaceSpecialamount(e);
        })
        $(".textrightamount").change(function () {
            
            var x = $(this);
            var str = parsenumeric($(this));//.val().trim();
            
            str = str.toString().replaceAll(',', '');
            var resultstr = '';
            if (str != '' && str != null) {
                //$(this).val($(this).val().trim());
                //str = $(this).val();
                //console.log(str);
                var _decimal = $('#hdncompanydecimal').val();
                var _numberformat = $('#hdnnumberformat').val();
                $(this).attr('ovalue', str);
                
                if (_numberformat=="Lakhs")
                    resultstr = getnumberformatLakhs(parseFloat(str).toFixed(_decimal));
                else
                    resultstr = addCommas(parseFloat(str).toFixed(_decimal));
                $(this).val(resultstr);
                //$(this).val(parseFloat(str).toFixed(_decimal));
            }
            else {
                $(this).val(parseFloat(0).toFixed(_decimal));
            }

        });

        $(".textright0").change(function () {
            var x = $(this);
            var str = $(this).val().trim();
            if (str != '' && str != null) {
                $(this).val($(this).val().trim());
                str = $(this).val();
                //console.log(str);
                $(this).val(parseFloat(str).toFixed(0));
            }
            else {
                $(this).val(parseFloat(0).toFixed(0));
            }

        });
        $(".textright3").change(function () {
            var x = $(this);
            var str = $(this).val().trim();
            if (str != '' && str != null) {
                $(this).val($(this).val().trim());
                str = $(this).val();
                //console.log(str);
                $(this).val(parseFloat(str).toFixed(3));
            }
            else {
                $(this).val(parseFloat(0).toFixed(3));
            }

        });
        $(".textright4").change(function () {
            var x = $(this);
            var str = $(this).val().trim();
            if (str != '' && str != null) {
                $(this).val($(this).val().trim());
                str = $(this).val();
                //console.log(str);
                $(this).val(parseFloat(str).toFixed(4));
            }
            else {
                $(this).val(parseFloat(0).toFixed(4));
            }

        });
        $('.caretauto').click(function () {
            var obj = $(this).next('.form-control');
            var str = ' ';
            if ($(obj).val() == null || $(obj).val() == '')
                str = ' ';
            else
                str = $(obj).val();

            $(obj).autocomplete('search', str);
            $(obj).focus();
        })
        $('.form-select').click(function () {
            var str = ' ';            
            if ($(this).val() == null || $(this).val() == '')
                str = ' ';
            else
                str = $(this).val();

            $(this).autocomplete('search', str);
        })

        //$.ajax({
        //    type: "Post",
        //    url: '/Menu/GetMenuAccess/',
        //    datatype: "json",
        //    data: { MenuTitle:$("#hdnPageTitle").val() },
        //    success: function (response) {
        //        debugger;
        //        if (response.status == "OK") {
        //            console.log(response);
                     
        //            var idtext = 'mdi-pencil'
        //            $('.mdi-pencil').each(function (index, item) {
        //                $(item).parent().removeClass('text-success');
        //                $(item).parent().addClass('text-secondary');
        //                $(item).parent().attr('href','javascript:void(0)');
        //            });
        //            $('.mdi-plus').each(function (index, item) {
        //                $(item).parent().removeClass('btn-primary');
        //                $(item).parent().addClass('btn-secondary');
        //                $(item).parent().attr('href', 'javascript:void(0)');
        //            });
        //            //var curSubmit =  $(window).closest('form').find(':submit');
        //            //alert($(curSubmit).val());
        //        }
        //        else {
                    
        //        }
        //    }
        //});

        $(".liyearitem").on('click', function (obj) {
            var id = $(this).attr('id').replace('liyear_', '');
            $.ajax({
                type: "POST",
                url: "/Home/ChangeYear",
                datatype: "Json",
                data: { id: id },
                success: function (response) {
                    if (response.status == "ok") {
                        //window.location.href = '@Session["HomePage"].ToString()' //  reload();
                        window.location.reload();
                        //$.notify(response.message, "success");
                        $("#spanyear").html(response.yearname);
                    }
                    else {
                        alert(response.message);

                    }
                }

            });

        });
        $(".lidepotitem").on('click', function (e) {
            debugger;
            var id = $(this).attr('id').replace('lidepot_', '');
            $.ajax({
                type: "POST",
                url: "/Home/ChangeBranch",
                datatype: "Json",
                data: { id: id },
                success: function (response) {
                    if (response.status == "ok") {
                        window.location.reload();
                        //window.location.href= '@Session["HomePage"].ToString()' //  reload();
                        $("#spandepot").html(response.depotname);
                        //$.notify(response.message, "success");
                    }
                    else {
                        $.notify(response.message, "error");

                    }
                }

            });
        });

    }
    $(document).ready(function () {
        init();
        $("input:text").focus(function () { $(this).select(); });
    })
    

})(jQuery)