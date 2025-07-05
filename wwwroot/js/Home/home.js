document.addEventListener("DOMContentLoaded", function () {
    const departureInput = document.getElementById("departureDateInput");
    const btnToday = document.getElementById("btnToday");
    const btnTomorrow = document.getElementById("btnTomorrow");
    
    // Set default date, use existing input value or default to tomorrow
    const defaultDate = departureInput.value
        ? departureInput.value
        : new Date(Date.now() + 24 * 60 * 60 * 1000);

    const fp = flatpickr(departureInput, {
        dateFormat: "d F Y",
        locale: window.currentFlatpickrLocale || "default",
        minDate: "today",
        defaultDate: defaultDate,
        onChange: updateTodayTomorrowButtons // Update button highlight on date change
    });

    btnToday.addEventListener("click", function () {
        const today = new Date();
        fp.setDate(today);
        updateActiveButton("btnToday");
    });

    btnTomorrow.addEventListener("click", function () {
        const tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        fp.setDate(tomorrow);
        updateActiveButton("btnTomorrow");
    });


    // Highlight the correct today/tomorrow button based on selected date
    function updateTodayTomorrowButtons(selectedDates) {
        const selected = selectedDates[0];
        const today = new Date();
        const tomorrow = new Date();
        tomorrow.setDate(today.getDate() + 1);

        const isToday = selected.toDateString() === today.toDateString();
        const isTomorrow = selected.toDateString() === tomorrow.toDateString();

        if (isToday) updateActiveButton("btnToday");
        else if (isTomorrow) updateActiveButton("btnTomorrow");
        else updateActiveButton(null);
    }


    // Apply active class to the correct button
    function updateActiveButton(activeId) {
        [btnToday, btnTomorrow].forEach(btn => btn.classList.remove("active"));
        if (activeId) document.getElementById(activeId).classList.add("active");
    }

    document.getElementById("swapButton").addEventListener("click", function () {
        const origin = document.getElementById("originInput");
        const originId = document.getElementById("OriginId");
        const dest = document.getElementById("destinationInput");
        const destId = document.getElementById("DestinationId");

        [origin.value, dest.value] = [dest.value, origin.value];
        [originId.value, destId.value] = [destId.value, originId.value];
    });
    
    // Validate form submission if origin and destination are the same
    document.querySelector(".submit-button").addEventListener("click", function (e) {
        const originId = document.getElementById("OriginId").value;
        const destinationId = document.getElementById("DestinationId").value;
        const errorDiv = document.getElementById("locationError");

        if (originId && destinationId && originId === destinationId) {
            e.preventDefault();
            errorDiv.style.display = "block";
        } else {
            errorDiv.style.display = "none";
        }
    });

    // If a date is already today/tomorrow, choose the appropriate button
    if (departureInput.value) {
        const parsed = fp.parseDate(departureInput.value, "d F Y");
        if (parsed) 
            updateTodayTomorrowButtons([parsed]);
    }
    
    // Select autocomplete elements
    const originInput = document.getElementById("originInput");
    const originIdInput = document.getElementById("OriginId");
    const originDropdown = document.getElementById("originDropdown");

    const destinationInput = document.getElementById("destinationInput");
    const destinationIdInput = document.getElementById("DestinationId");
    const destinationDropdown = document.getElementById("destinationDropdown");

    setupAutocomplete(originInput, originIdInput, originDropdown);
    setupAutocomplete(destinationInput, destinationIdInput, destinationDropdown);
    
    // Setup autocomplete dropdown functionality
    function setupAutocomplete(inputEl, hiddenIdEl, dropdownEl) {
        let currentIndex = -1;
        let debounceTimer;

        inputEl.addEventListener("input", function () {
            const query = this.value.trim();
            dropdownEl.innerHTML = "";
            currentIndex = -1;

            clearTimeout(debounceTimer);
            if (!query) {
                dropdownEl.style.display = "none";
                return;
            }

            debounceTimer = setTimeout(() => {
                fetch("/Home/SearchLocations", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(query)
                })
                    .then(res => res.json())
                    .then(locations => {
                        if (!locations.length) {
                            dropdownEl.style.display = "none";
                            return;
                        }

                        locations.forEach((loc, i) => {
                            const li = document.createElement("li");
                            li.textContent = loc.text;
                            li.dataset.id = loc.value;
                            li.setAttribute("data-index", i);
                            li.addEventListener("click", function () {
                                inputEl.value = loc.text;
                                hiddenIdEl.value = loc.value;
                                dropdownEl.innerHTML = "";
                                dropdownEl.style.display = "none";
                            });
                            dropdownEl.appendChild(li);
                        });

                        dropdownEl.style.display = "block";
                    })
                    .catch(console.error);
            }, 300);
        });

        inputEl.addEventListener("keydown", function (e) {
            const items = dropdownEl.querySelectorAll("li");
            if (dropdownEl.style.display !== "block" || items.length === 0) return;

            if (e.key === "ArrowDown") {
                e.preventDefault();
                currentIndex = (currentIndex + 1) % items.length;
                updateActiveItem(items);
            } else if (e.key === "ArrowUp") {
                e.preventDefault();
                currentIndex = (currentIndex - 1 + items.length) % items.length;
                updateActiveItem(items);
            } else if (e.key === "Enter") {
                if (currentIndex >= 0 && items[currentIndex]) {
                    e.preventDefault();
                    items[currentIndex].click();
                }
            }
        });

        function updateActiveItem(items) {
            items.forEach((item, idx) => {
                item.classList.toggle("active", idx === currentIndex);
                if (idx === currentIndex) {
                    item.scrollIntoView({ block: "nearest" });
                }
            });
        }

        document.addEventListener("click", function (e) {
            if (!inputEl.contains(e.target) && !dropdownEl.contains(e.target)) {
                dropdownEl.style.display = "none";
            }
        });
    }
});