document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("vehicleSearchForm");
    const makeSelect = document.getElementById("makeSelect");
    const $form = window.jQuery(form);
    const vehicleTypeSelect = document.getElementById("vehicleTypeSelect");
    const errorContainer = document.getElementById("vehicleTypeLoadError");
    const searchButton = document.getElementById("searchButton");
    if (!form || !makeSelect || !vehicleTypeSelect || !errorContainer || !searchButton) {
        return;
    }

    const $makeSelect = window.jQuery(makeSelect);

    if (typeof $makeSelect.select2 === "function") {
        $makeSelect.select2({
            placeholder: "Search or select a car make",
            allowClear: true,
            width: "100%"
        });
    }

    const vehicleTypesUrl = form.dataset.vehicleTypesUrl;

    if (!vehicleTypesUrl) {
        errorContainer.textContent = "Vehicle types service URL is not configured.";

        vehicleTypeSelect.disabled = true;
        return;
    }

    let activeRequestController = null;

    function resetVehicleTypes(message) {
        vehicleTypeSelect.replaceChildren();

        const option = document.createElement("option");
        option.value = "";
        option.textContent = message;

        vehicleTypeSelect.appendChild(option);
    }

    function setLoadingState() {
        errorContainer.textContent = "";
        resetVehicleTypes("Loading vehicle types...");
        vehicleTypeSelect.disabled = true;
    }

    function setEmptyState() {
        resetVehicleTypes("No vehicle types were found");
        vehicleTypeSelect.disabled = true;
    }

    function setErrorState(message) {
        resetVehicleTypes("Vehicle types are unavailable");
        vehicleTypeSelect.disabled = true;
        errorContainer.textContent = message;
    }

    async function loadVehicleTypes(makeId, selectedValue = "") {
        activeRequestController?.abort();

        const requestController = new AbortController();
        activeRequestController = requestController;

        setLoadingState();

        try {
            const requestUrl = `${vehicleTypesUrl}?makeId=${encodeURIComponent(makeId)}`;

            const response = await fetch(requestUrl, {
                method: "GET",
                headers: {
                    Accept: "application/json"
                },
                signal: requestController.signal
            });

            if (!response.ok) {
                const responseBody = await response
                    .json()
                    .catch(() => null);

                throw new Error(responseBody?.message ?? "Unable to load vehicle types.");
            }

            const vehicleTypes = await response.json();

            if (!Array.isArray(vehicleTypes) || vehicleTypes.length === 0) {
                setEmptyState();
                return;
            }

            resetVehicleTypes("Select a vehicle type");

            vehicleTypes.forEach(vehicleType => {
                const vehicleTypeName = vehicleType.vehicleTypeName ?? vehicleType.VehicleTypeName;

                if (!vehicleTypeName) {
                    return;
                }

                const option = document.createElement("option");

                option.value = vehicleTypeName;
                option.textContent = vehicleTypeName;
                option.selected = vehicleTypeName === selectedValue;

                vehicleTypeSelect.appendChild(option);
            });

            vehicleTypeSelect.disabled = false;
        }
        catch (error) {
            if (error instanceof DOMException && error.name === "AbortError") {
                return;
            }

            setErrorState(error instanceof Error ? error.message : "Unable to load vehicle types.");
        }
        finally {
            if (activeRequestController === requestController) {
                activeRequestController = null;
            }
        }
    }

    $makeSelect.on("change", async function () {
        const makeId = this.value;

        const validator = $form.data("validator");
        if (validator) {
            validator.settings.ignore = ":hidden:not(.select2-hidden-accessible)";
        }

        if (typeof $makeSelect.valid === "function") {
            $makeSelect.valid();
        }

        if (!makeId) {
            activeRequestController?.abort();

            resetVehicleTypes("Select a car make first");
            vehicleTypeSelect.disabled = true;
            errorContainer.textContent = "";

            return;
        }

        await loadVehicleTypes(makeId);
    });

    form.addEventListener("submit", event => {

        if (typeof $form.valid === "function" &&
            !$form.valid()) {
            return;
        }

        if (form.dataset.isSubmitting === "true") {
            event.preventDefault();
            return;
        }

        form.dataset.isSubmitting = "true";

        searchButton.disabled = true;
        searchButton.textContent = "Searching...";
    });

    const hasServerVehicleTypes = vehicleTypeSelect.options.length > 1;

    if (!makeSelect.value) {
        resetVehicleTypes("Select a car make first");
        vehicleTypeSelect.disabled = true;
        return;
    }

    if (!hasServerVehicleTypes) {
        const selectedVehicleType = vehicleTypeSelect.dataset.selectedValue ?? "";

        void loadVehicleTypes(makeSelect.value, selectedVehicleType);
    }

});