//// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
//// for details on configuring this project to bundle and minify static web assets.

//// Write your JavaScript code.

//var observer = new MutationObserver(() => {
//console.log("Change observed!");
//    if (localStorage.getItem("selectedAlert") != null) {
//        selectAlert(localStorage.getItem("selectedAlert"), localStorage.getItem("selectedInputs"), localStorage.getItem("selectedId"), localStorage.getItem("selectedType"),
//            localStorage.getItem("selectedLat"), localStorage.getItem("selectedLon"), localStorage.getItem("selectedTime"), localStorage.getItem("selectedAccuracy"),
//            localStorage.getItem("selectedInfo"), localStorage.getItem("selectedFrame"));
//    }
//});

//observer.observe(document.getElementsByTagName("table")[0], { childList: true, subtree: true });

//const selectAlert = (that, inputs, id, type, lat, lon, time, accuracy, info, frame) => {
//    $("td").removeClass("selected-alert");
//    $(that).find("td").addClass("selected-alert");
//    if (inputs == "both") {
//        $("#info-content").html(`
//                <div class="info-container d-flex flex-column justify-content-between">
//                    <img src="data:image/jpeg;base64,${frame}" alt="Alert Information Image" class="alert-image">
//                    <div class="alert-description border border-dark position-relative rounded overflow-auto">
//                        <pre class="alert-text position-absolute top-0 left-0 font-sans p-1">
//    @*Lorem ipsum dolor sit amet consectetur adipisicing elit. Veritatis, quas beatae neque error quasi dolores minima iste tempora officiis harum nisi placeat. Sit laboriosam perferendis iure quisquam nobis voluptates libero!
//Dolor eos, eum consectetur, odio quas eius officiis modi illo incidunt ex amet! Impedit, autem accusantium at quam voluptatum iste est deserunt magni. Voluptates enim ut explicabo nesciunt optio cumque.
//Labore velit ipsum dolores debitis deleniti eveniet. Distinctio pariatur nulla, harum excepturi ipsa iste consectetur error et aspernatur reprehenderit beatae atque quo necessitatibus repudiandae vitae velit odit ad architecto! Possimus?
//Itaque dolor reiciendis, voluptatum iusto repellendus repudiandae nihil beatae. Nam itaque molestiae excepturi quam cumque dolores libero similique iusto nemo! Esse reiciendis exercitationem rem temporibus et non similique nostrum quaerat.*@
//<strong>Alert: ${id}</strong><br>
//<strong>${type}</strong><br>
//&emsp; – Position: (${lat},${lon})<br>
//&emsp; - Day/Time: ${time}<br>
//&emsp; - Detection Confidence: ${accuracy} %<br>
//${info}
//                        </pre>
//                    </div>
//                </div>
//            `);
//    } else if (inputs == "text") {
//        $("#info-content").html(`
//                <div class="border border-dark position-relative rounded overflow-auto" id="text-selected">
//                    <pre class="alert-text position-absolute top-0 left-0 font-sans p-1">
//    @*Lorem ipsum dolor sit amet consectetur adipisicing elit. Veritatis, quas beatae neque error quasi dolores minima iste tempora officiis harum nisi placeat. Sit laboriosam perferendis iure quisquam nobis voluptates libero!
//Dolor eos, eum consectetur, odio quas eius officiis modi illo incidunt ex amet! Impedit, autem accusantium at quam voluptatum iste est deserunt magni. Voluptates enim ut explicabo nesciunt optio cumque.
//Labore velit ipsum dolores debitis deleniti eveniet. Distinctio pariatur nulla, harum excepturi ipsa iste consectetur error et aspernatur reprehenderit beatae atque quo necessitatibus repudiandae vitae velit odit ad architecto! Possimus?
//Itaque dolor reiciendis, voluptatum iusto repellendus repudiandae nihil beatae. Nam itaque molestiae excepturi quam cumque dolores libero similique iusto nemo! Esse reiciendis exercitationem rem temporibus et non similique nostrum quaerat.*@
//<strong>Alert: ${id}</strong><br>
//<strong>${type}</strong><br>
//&emsp; – Position: (${lat},${lon})<br>
//&emsp; - Day/Time: ${time}<br>
//&emsp; - Detection Confidence: ${accuracy} %<br>
//${info}
//                    </pre>
//                </div>
//            `);
//    } else {
//        $("#info-content").html(`
//                <div class="info-container d-flex flex-column justify-content-center">
//                    <img src="data:image/jpeg;base64,${frame}" alt="Alert Information Image" class="alert-image">
//                </div>
//            `);
//    }
//    localStorage.setItem("selectedAlert", that);
//    localStorage.setItem("selectedInputs", inputs);
//    localStorage.setItem("selectedId", id);
//    localStorage.setItem("selectedType", type);
//    localStorage.setItem("selectedLat", lat);
//    localStorage.setItem("selectedLon", lon);
//    localStorage.setItem("selectedTime", time);
//    localStorage.setItem("selectedAccuracy", accuracy);
//    localStorage.setItem("selectedInfo", info);
//    localStorage.setItem("selectedFrame", frame);
//}

//const toggleAlertFilter = () => {
//    if ($("#alert-filter").attr("class").includes("hide")) {
//        $("#alert-filter").removeClass("hide");
//        $("#alert-filter").addClass("show");
//    }
//    else {
//        $("#alert-filter").removeClass("show");
//        $("#alert-filter").addClass("hide");
//    }
//}

//const toggleSourceFilter = () => {
//    if ($("#source-filter").attr("class").includes("hide")) {
//        $("#source-filter").removeClass("hide");
//        $("#source-filter").addClass("show");
//    }
//    else {
//        $("#source-filter").removeClass("show");
//        $("#source-filter").addClass("hide");
//    }
//}

//const applyFilters = (source) => {
//    let alertCheckboxes = $("#alert-filter").find("input");
//    let alertLabels = ["danger", "warning", "info"]
//    let alertFilter = {};
//    for (let i = 0; i < alertCheckboxes.length; i++) {
//        alertFilter[alertLabels[i]] = alertCheckboxes[i].checked;
//    }
//    let sourceCheckboxes = $("#source-filter").find("input");
//    let sourceLabels = $("#source-filter").find("label");
//    let sourceFilter = {};
//    for (let i = 0; i < sourceCheckboxes.length; i++) {
//        sourceFilter[sourceLabels[i].textContent] = sourceCheckboxes[i].checked;
//    }
//    let rows = $("table").find("tr");
//    for (let i = 0; i < rows.length; i++) {
//        let count = 0
//        let data = rows[i].querySelectorAll("td");
//        for (let j = 0; j < data.length; j++) {
//            if (Object.keys(sourceFilter).includes(data[j].textContent) || Object.keys(alertFilter).includes(data[j].textContent.trim().split("-")[1])) {
//                rows[i].classList.add("hide");
//                if (sourceFilter[data[j].textContent] || alertFilter[data[j].textContent.trim().split("-")[1]]) {
//                    count++;
//                }
//            }
//        }
//        if (count == 2) {
//            rows[i].classList.remove("hide");
//        }
//    }
//    if (source == "alert") {
//        toggleAlertFilter();
//    } else if (source == "source") {
//        toggleSourceFilter();
//    }
//    localStorage.setItem("sourceFilter", sourceFilter);
//    localStorage.setItem("alertFilter", alertFilter);
//}