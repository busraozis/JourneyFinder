document.addEventListener("DOMContentLoaded", function () {
    const userLang = navigator.language || navigator.userLanguage;
    const isTurkish = userLang.toLowerCase().startsWith("tr");

    const locale = isTurkish ? flatpickr.l10ns.tr : flatpickr.l10ns.default;

    const departureInput = document.getElementById("departureDateInput");
    const btnToday = document.getElementById("btnToday");
    const btnTomorrow = document.getElementById("btnTomorrow");
    const swapButton = document.getElementById("swapButton");
    const submitButton = document.querySelector(".submit-button");
    const locationError = document.getElementById("locationError");

    const originInput = document.getElementById("originInput");
    const originIdInput = document.getElementById("OriginId");
    const originDropdown = document.getElementById("originDropdown");

    const destinationInput = document.getElementById("destinationInput");
    const destinationIdInput = document.getElementById("DestinationId");
    const destinationDropdown = document.getElementById("destinationDropdown");

    if (!window.locations) {
        console.warn("locations değişkeni bulunamadı.");
        return;
    }

    const defaultDate = departureInput.value || new Date(Date.now() + 24 * 60 * 60 * 1000);

    const fp = flatpickr(departureInput, {
        dateFormat: "d F Y",
        locale: locale,
        minDate: "today",
        defaultDate: defaultDate,
        onChange: updateTodayTomorrowButtons
    });

    btnToday.addEventListener("click", () => {
        const today = new Date();
        fp.setDate(today);
        updateActiveButton("btnToday");
    });

    btnTomorrow.addEventListener("click", () => {
        const tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        fp.setDate(tomorrow);
        updateActiveButton("btnTomorrow");
    });

    function updateTodayTomorrowButtons(selectedDates) {
        const selected = selectedDates[0];
        const today = new Date();
        const tomorrow = new Date();
        tomorrow.setDate(today.getDate() + 1);

        if (selected.toDateString() === today.toDateString()) updateActiveButton("btnToday");
        else if (selected.toDateString() === tomorrow.toDateString()) updateActiveButton("btnTomorrow");
        else updateActiveButton(null);
    }

    function updateActiveButton(activeId) {
        [btnToday, btnTomorrow].forEach(btn => btn.classList.remove("active"));
        if (activeId) document.getElementById(activeId).classList.add("active");
    }

    swapButton.addEventListener("click", () => {
        [originInput.value, destinationInput.value] = [destinationInput.value, originInput.value];
        [originIdInput.value, destinationIdInput.value] = [destinationIdInput.value, originIdInput.value];
    });

    submitButton.addEventListener("click", function (e) {
        const originId = originIdInput.value;
        const destinationId = destinationIdInput.value;

        if (originId && destinationId && originId === destinationId) {
            e.preventDefault();
            locationError.style.display = "block";
        } else {
            locationError.style.display = "none";
        }
    });

    if (departureInput.value) {
        const parsed = fp.parseDate(departureInput.value, "d F Y");
        if (parsed) updateTodayTomorrowButtons([parsed]);
    }

    setupDropdown(originInput, originIdInput, originDropdown);
    setupDropdown(destinationInput, destinationIdInput, destinationDropdown);

    function setupDropdown(inputEl, hiddenIdEl, dropdownEl) {
        let currentIndex = -1;

        inputEl.addEventListener("input", function () {
            const query = this.value.toLowerCase();
            dropdownEl.innerHTML = "";
            currentIndex = -1;

            const filtered = window.locations.filter(loc => loc.Text.toLowerCase().includes(query));
            if (!filtered.length) return dropdownEl.style.display = "none";

            filtered.forEach((loc, i) => {
                const li = document.createElement("li");
                li.textContent = loc.Text;
                li.dataset.id = loc.Value;
                li.setAttribute("data-index", i);
                li.addEventListener("click", () => {
                    inputEl.value = loc.Text;
                    hiddenIdEl.value = loc.Value;
                    dropdownEl.innerHTML = "";
                    dropdownEl.style.display = "none";
                });
                dropdownEl.appendChild(li);
            });

            dropdownEl.style.display = "block";
        });

        inputEl.addEventListener("keydown", function (e) {
            const items = dropdownEl.querySelectorAll("li");
            if (dropdownEl.style.display !== "block" || !items.length) return;

            if (e.key === "ArrowDown") {
                e.preventDefault();
                currentIndex = (currentIndex + 1) % items.length;
                updateActiveItem(items);
            } else if (e.key === "ArrowUp") {
                e.preventDefault();
                currentIndex = (currentIndex - 1 + items.length) % items.length;
                updateActiveItem(items);
            } else if (e.key === "Enter" && currentIndex >= 0) {
                e.preventDefault();
                items[currentIndex].click();
            }
        });

        document.addEventListener("click", function (e) {
            if (!inputEl.contains(e.target) && !dropdownEl.contains(e.target)) {
                dropdownEl.style.display = "none";
            }
        });

        function updateActiveItem(items) {
            items.forEach((item, idx) => {
                item.classList.toggle("active", idx === currentIndex);
                if (idx === currentIndex) item.scrollIntoView({ block: "nearest" });
            });
        }
    }
});
