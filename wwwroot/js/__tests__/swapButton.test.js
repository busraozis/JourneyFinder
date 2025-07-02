/**
 * @jest-environment jsdom
 */

describe("Swap button functionality", () => {
    let document, originInput, destinationInput, originId, destinationId, swapButton;

    beforeEach(() => {
        document = new DOMParser().parseFromString(`
      <input id="originInput" value="Ankara" />
      <input id="OriginId" value="34" />
      <input id="destinationInput" value="Istanbul" />
      <input id="DestinationId" value="06" />
      <button id="swapButton"></button>
    `, 'text/html');

        global.document = document;

        originInput = document.getElementById("originInput");
        destinationInput = document.getElementById("destinationInput");
        originId = document.getElementById("OriginId");
        destinationId = document.getElementById("DestinationId");
        swapButton = document.getElementById("swapButton");

        swapButton.addEventListener("click", function () {
            [originInput.value, destinationInput.value] = [destinationInput.value, originInput.value];
            [originId.value, destinationId.value] = [destinationId.value, originId.value];
        });
    });

    test("it swaps input values and Ids", () => {
        swapButton.click();

        expect(originInput.value).toBe("Istanbul");
        expect(destinationInput.value).toBe("Ankara");
        expect(originId.value).toBe("06");
        expect(destinationId.value).toBe("34");
    });
});
