import {env} from "std-env";

export default defineEventHandler((event) => {
    return env.BACKEND_HOST
})