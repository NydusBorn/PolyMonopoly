import { test, expect } from "@playwright/test";

test.describe("login page", () => {
  test("has title", async ({ page }) => {
    await page.goto("http://localhost:3000/login");

    await expect(page).toHaveTitle("PolyMonopoly - Login");
  });

  test("guest register", async ({ page }) => {
    await page.goto("http://localhost:3000/login");
    await page.waitForLoadState("networkidle");

    await page.fill("#username", "guest");

    await page.click("#guest_button");

    await expect(page).toHaveURL("http://localhost:3000/lobby");
    let lcp = await page.context().storageState();
    let lca = lcp.origins[0].localStorage;
    let lc = {};
    lca.forEach((x) => {
      if (x.name.includes(".")) {
        const parts = x.name.split(".");
        if (parts.length >= 2) {
          if (lc[parts[0]] === undefined) {
            lc[parts[0]] = {};
          }
          if (parts.length === 2) {
            lc[parts[0]][parts[1]] = x.value;
          }
        }
      } else {
        lc[x.name] = x.value;
      }
    });
    expect(lc).toHaveProperty("user.uid");

    expect(lc).toHaveProperty("user.password");
    expect(lc["user"]["password"].length).toBe(16);
  });

  test("user register", async ({ page }) => {
    await page.goto("http://localhost:3000/login");
    await page.waitForLoadState("networkidle");

    await page.fill("#username", "user");
    await page.fill(".p-password-input", "testpass");

    await page.click("#user_button");

    await expect(page).toHaveURL("http://localhost:3000/lobby");
    let lcp = await page.context().storageState();
    let lca = lcp.origins[0].localStorage;
    let lc = {};
    lca.forEach((x) => {
      if (x.name.includes(".")) {
        const parts = x.name.split(".");
        if (parts.length >= 2) {
          if (lc[parts[0]] === undefined) {
            lc[parts[0]] = {};
          }
          if (parts.length === 2) {
            lc[parts[0]][parts[1]] = x.value;
          }
        }
      } else {
        lc[x.name] = x.value;
      }
    });
    expect(lc).toHaveProperty("user.uid");

    expect(lc).toHaveProperty("user.password");
    expect(lc["user"]["password"].length).toBe(8);
  });
});
