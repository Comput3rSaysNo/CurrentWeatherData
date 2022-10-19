<template>
    <div class="home card mt-5">

        <h1 class="mb-5 text-center">Current Weather Data Api Test Client</h1>

        <div class="mb-3">
            <label for="country" class="form-label">Country</label>
            <input id="country" type="text" v-model="country" class="form-control" />

        </div>
        <div class="mb-3">
            <label for="city" class="form-label">City</label>
            <input id="city" type="text" v-model="city" class="form-control" />
        </div>
        <div class="mb-3">
            <label for="apikey" class="form-label">Api Key</label>
            <select id="apikey" class="form-control" v-model="apiKey" :options="apiKeyList">
                <option v-for="k in apiKeyList" :value="k">
                    {{ k }}
                </option>
            </select>
        </div>

        <div class="mb-5">
            <label for="output" class="form-label">Response</label>
            <textarea id="output" class="form-control" readonly disabled style="height: 300px; resize: none;" :value="result"></textarea>
            <div id="responseHelper" class="form-text">stack trace info is only available in DEV environment.</div>
        </div>

        
        <button class="btn btn-primary p-3" @click="queryCurrentWeatherDataApi" :disabled="loading">{{loading ? "Loading..." :"Query"}}</button>

    </div>
</template>

<script>
    import axios from 'axios';

    export default {
        name: 'Current Weather Data Api Test Client',
        props: {
        },
        data() {
            return {
                apiKey: "4d4a1b16-cf93-4800-83a6-e4e46725ea6a",
                result: "",
                city: "Melbourne",
                country: "Australia",
                apiKeyList: [
                    "4d4a1b16-cf93-4800-83a6-e4e46725ea6a",
                    "b9ff4e01-53a9-4485-928d-0b99e2bd9816",
                    "90fbd061-19e5-4de4-8339-436a6acbf286",
                    "cdc73f3b-0af8-4575-98ed-ad334240e25a",
                    "8d1caf6f-0f5c-4819-ba6b-8ed92e3f7140"
                ],
                loading: false,

            }
        },
        methods: {
            queryCurrentWeatherDataApi() {
                this.loading = true;
                axios.get('https://localhost:44313/weather?Country=' + encodeURIComponent(this.country) + '&city=' + encodeURIComponent(this.city) + '&api-key=' + encodeURIComponent(this.apiKey))
                    .then(response => {
                        this.result = JSON.stringify(response.data);
                    })
                    .catch(error => {
                        try {
                            this.result = JSON.stringify(error.response.data);
                        }
                        catch (err) {
                            this.result = JSON.stringify(error);
                        }
                        
                    })
                    .then(() => {
                        setTimeout(() => { this.loading = false }, 1000);
                    });
            }
        }
    }

</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
    
    .home{
        width:800px;
        border:1px solid #ccc;
        padding: 50px 20px;
        margin:0 auto;
    }
</style>
