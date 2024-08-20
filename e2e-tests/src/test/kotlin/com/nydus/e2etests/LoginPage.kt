package com.nydus.e2etests;

import com.microsoft.playwright.Browser
import com.microsoft.playwright.BrowserContext
import com.microsoft.playwright.Page
import com.microsoft.playwright.Playwright

import org.testng.annotations.*

import org.testng.Assert.assertEquals
import org.testng.Assert.assertTrue
import java.io.File
import java.lang.ProcessBuilder

class LoginPage {
    lateinit var page: Page
    lateinit var context: BrowserContext

    companion object {
        lateinit var playwright: Playwright
        lateinit var browser: Browser
        lateinit var aspServer: Process
        lateinit var nuxtServer: Process

        @JvmStatic
        @BeforeClass
        fun preparation() {
            aspServer = ProcessBuilder("dotnet", "run").directory(File("../asp-backend/asp-backend")).start()
            val t = ProcessBuilder("npm", "run", "dev").directory(File("../web-client"))
            val tenv = t.environment()
            tenv["BACKEND_HOST"] = "http://localhost:5220"
            tenv["NUXT_HOST"] = ""
            nuxtServer = t.start()
            Thread.sleep(2000)
            playwright = Playwright.create()
            browser = playwright.chromium().launch()
        }

        @JvmStatic
        @AfterClass
        fun teardown() {
            playwright.close()
            aspServer.destroy()
            nuxtServer.destroy()
        }
    }

    @BeforeMethod
    fun createContextAndPage() {
        context = browser.newContext()
        page = context.newPage()
        page.navigate("http://localhost:3000/login")
    }

    @AfterMethod
    fun closeContext() {
        context.close()
    }

    @Test
    fun shouldOpen() {
        assertEquals(page.title(), "PolyMonopoly - Login")
    }
}