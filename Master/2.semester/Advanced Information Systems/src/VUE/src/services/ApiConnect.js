import axios from 'axios'
import serviceConfig from "@/services/serviceConfig";

const headers = {
    'Content-Type': 'application/json'
}

export default axios.create({
    baseURL: serviceConfig.hostname,
    timeout: 5000,
    headers: headers
})