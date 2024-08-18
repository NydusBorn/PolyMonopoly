package com.nydus.e2etests;

import com.microsoft.playwright.Browser
import com.microsoft.playwright.BrowserContext
import com.microsoft.playwright.Page
import com.microsoft.playwright.Playwright

import org.testng.annotations.*

import org.testng.Assert.assertEquals
import org.testng.Assert.assertTrue

class TestExample {
    lateinit var page: Page
    lateinit var context: BrowserContext

    companion object {
        lateinit var playwright: Playwright
        lateinit var browser: Browser

        @JvmStatic
        @BeforeClass
        fun launchBrowser() {
            playwright = Playwright.create()
            browser = playwright.chromium().launch()
        }

        @JvmStatic
        @AfterClass
        fun closeBrowser() {
            playwright.close()
        }
    }

    @BeforeMethod
    fun createContextAndPage() {
        context = browser.newContext()
        page = context.newPage()
    }

    @AfterMethod
    fun closeContext() {
        context.close()
    }

    @Test
    fun shouldClickButton() {
        page.navigate("data:text/html,<script>var result;</script><button onclick='result=\"Clicked\"'>Go</button>")
        page.locator("button").click()
        assertEquals(page.evaluate("result"), "Clicked")
    }

    @Test
    fun shouldCheckTheBox() {
        page.setContent("<input id='checkbox' type='checkbox'></input>")
        page.locator("input").check()
        assertTrue((page.evaluate("() => window['checkbox'].checked") as Boolean))
    }

    @Test
    fun shouldSearchWiki() {
        page.navigate("https://www.wikipedia.org/")
        page.locator("input[name=\"search\"]").click()
        page.locator("input[name=\"search\"]").fill("playwright")
        page.locator("input[name=\"search\"]").press("Enter")
        assertEquals(page.url(), "https://en.wikipedia.org/wiki/Playwright")
    }
}