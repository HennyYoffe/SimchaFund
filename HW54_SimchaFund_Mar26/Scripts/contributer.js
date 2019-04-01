$(() => {
    $("#new-contributor").on('click', function () {
        $("#addcontributer").modal();
    });
    $(".deposit").on('click', function () {
        const button = $(this);
        const id = button.data('id');       
        $("#contributerid").val(`${ id }`);
            $("#adddeposit").modal();
    });
    $(".edit").on('click', function () {
        const button = $(this);
        const id = button.data('id');     
        const fName = button.data('first-name'); 
        const lname = button.data('last-name'); 
        const cell = button.data('cell'); 
        const ainclude = button.data('always-include'); 
        var date = button.data('date-created');
        $("#eid").val(`${id}`);
        $("#efirstname").val(`${fName}`);
        $("#elastname").val(`${lname}`);
        $("#ecell").val(`${cell}`);
        $("#edate").val(`${date}`);    
        if (ainclude == true) {
            document.getElementById("ealwaysinclude").checked = true;
        }
        $("#editcontributer").modal();

      
       
       

    });
});
//<article
//    id="electric-cars"
//    data-columns="3"
//    data-index-number="12314"
//    data-parent="cars">
//    ...
//</article>

//const article = document.querySelector('#electric-cars');

//article.dataset.columns // "3"
//article.dataset.indexNumber // "12314"
//article.dataset.parent 

//<button class="btn btn-secondary edit" id="edit-contributer-id" data-id="@c.Id" data-first-name="@c.FirstName"
//    data-last-name="@c.LastName" data-cell="@c.Cell" data-always-include="@c.AlwaysInclude"
//    data-date-created="@c.Date.ToString(" yyyy-MM-dd")" >
//        Edit
//                            </button >