function color_picker_set_Style(selector, color, property) {
  document.querySelectorAll(selector).forEach(e => { e.style[property] = color })
}

// Example data
const color_picker_data = JSON.parse(localStorage.getItem("colorPicker")) || [
  ["Text", "white", "color", ".service-stats .scoreboard-text"],
  ["Icons", "white", "color", ".service-stats .scoreboard-icon"],

  ["service-OFFLINE",        "#800000", "background-color",".service-OFFLINE"],
  ["service-INTERNAL_ERROR", "#aaa", "background-color",".service-INTERNAL_ERROR"],
  ["service-OK",             "#158000", "background-color",".service-OK"],
  ["service-MUMBLE",         "#805500", "background-color",".service-MUMBLE"],
  ["service-RECOVERING",     "#004080", "background-color",".service-RECOVERING"],
];

// Menu
const color_picker_menu = document.createElement("div");
Object.assign(color_picker_menu.style, {
  position: "fixed",
  bottom: "85px",
  right: "20px",
  background: "#fff",
  border: "1px solid #ccc",
  borderRadius: "10px",
  padding: "12px",
  display: "block",
  zIndex: "999999",
  boxShadow: "0 4px 12px rgba(0,0,0,.2)",
  fontFamily: "sans-serif",
  width: "180px",
  color: "black"
});

// Button
const color_picker_button = document.createElement("button");
color_picker_button.textContent = "🎨";
Object.assign(color_picker_button.style, {
  position: "fixed",
  bottom: "20px",
  right: "20px",
  width: "56px",
  height: "56px",
  borderRadius: "50%",
  border: "none",
  background: "#333",
  color: "white",
  fontSize: "24px",
  cursor: "pointer",
  zIndex: "999999",
  boxShadow: "0 4px 10px rgba(0,0,0,.3)"
});

color_picker_data.forEach(([label, color, property, selector], index) => {
  const row = document.createElement("div");
  row.style.marginBottom = "10px";

  const text = document.createElement("div");
  text.textContent = label;
  text.style.fontSize = "13px";
  text.style.marginBottom = "4px";

  const input = document.createElement("input");
  input.type = "color";

  // Convert named colors to hex if needed
  const temp = document.createElement("div");
  temp.style.color = color;
  document.body.appendChild(temp);
  const rgb = getComputedStyle(temp).color;
  temp.remove();

  const match = rgb.match(/\d+/g);
  if (match) {
    input.value =
      "#" +
      match
        .slice(0, 3)
        .map(n => Number(n).toString(16).padStart(2, "0"))
        .join("");
  }

  input.oninput = () => {
    color_picker_data[index][1] = input.value; // Store the selected color
    localStorage.setItem("colorPicker",JSON.stringify(color_picker_data))
    color_picker_set_Style(color_picker_data[index][3], color_picker_data[index][1], color_picker_data[index][2])
  };

  row.appendChild(text);
  row.appendChild(input);
  color_picker_menu.appendChild(row);
});

document.body.appendChild(color_picker_menu);
color_picker_menu.style.display = "none";
document.body.appendChild(color_picker_button);
color_picker_data.forEach(([label, color, property, selector], index) => {
  color_picker_set_Style(color_picker_data[index][3], color_picker_data[index][1], color_picker_data[index][2])
});

function color_picker_updateVisibility() {
    const visible = location.pathname === "/scoreboard";
    color_picker_button.style.display = visible ? "block" : "none";
    if (!visible) {
        color_picker_menu.style.display = "none";
    }
}

window.addEventListener("popstate", color_picker_updateVisibility);

const pushState = history.pushState;
history.pushState = function (...args) {
    pushState.apply(this, args);
    color_picker_updateVisibility();
};

const replaceState = history.replaceState;
history.replaceState = function (...args) {
    replaceState.apply(this, args);
    color_picker_updateVisibility();
};

color_picker_button.onclick = () => {
  color_picker_menu.style.display = (color_picker_menu.style.display === "none" ) ? "block" : "none";
};

document.addEventListener("click", e => {
  if (!color_picker_menu.contains(e.target) && e.target !== color_picker_button) {
    color_picker_menu.style.display = "none";
  }
});