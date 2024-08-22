import { test, expect } from "@playwright/test";

test.describe("login page", () => {
  test("has title", async ({ page }) => {
    await page.goto("http://localhost:3000/login");

    await expect(page).toHaveTitle("PolyMonopoly - Login");
  });

  test("guest register", async ({ page }) => {
    await page.goto("http://localhost:3000/login");
    await page.waitForLoadState("networkidle");

    await page.fill("#username", "test");

    await page.click("#guest_button");

    await expect(page).toHaveURL("http://localhost:3000/lobby");
  });
});
