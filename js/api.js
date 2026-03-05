const API_BASE_URL = 'http://localhost:5174/api';

const CricketAPI = {
    async handleResponse(response) {
        if (!response.ok) {
            const text = await response.text();
            throw new Error(`API Error ${response.status}: ${text.substring(0, 100)}`);
        }
        const contentType = response.headers.get('content-type');
        if (!contentType || !contentType.includes('application/json')) {
            throw new Error('API returned non-JSON response');
        }
        return await response.json();
    },

    async getLiveMatches() {
        try {
            const response = await fetch(`${API_BASE_URL}/matches/live`);
            return await this.handleResponse(response);
        } catch (error) {
            console.error('Error fetching live matches:', error);
            return [];
        }
    },

    async getUpcomingSchedules() {
        try {
            const response = await fetch(`${API_BASE_URL}/schedules/upcoming`);
            return await this.handleResponse(response);
        } catch (error) {
            console.error('Error fetching schedules:', error);
            return [];
        }
    },

    async getBlogs() {
        try {
            const response = await fetch(`${API_BASE_URL}/blogs`);
            return await this.handleResponse(response);
        } catch (error) {
            console.error('Error fetching blogs:', error);
            return [];
        }
    },

    async searchPlayers(query) {
        try {
            const response = await fetch(`${API_BASE_URL}/players/search-api?name=${query}`);
            return await this.handleResponse(response);
        } catch (error) {
            console.error('Error searching players:', error);
            return [];
        }
    },

    async getSeriesInfo(id) {
        try {
            const response = await fetch(`${API_BASE_URL}/schedules/series/${id}`);
            return await this.handleResponse(response);
        } catch (error) {
            console.error('Error fetching series info:', error);
            return null;
        }
    },

    async getMatchInfo(id) {
        try {
            const response = await fetch(`${API_BASE_URL}/matches/${id}`);
            return await this.handleResponse(response);
        } catch (error) {
            console.error('Error fetching match info:', error);
            return null;
        }
    },

    async getCricScores() {
        try {
            const response = await fetch(`${API_BASE_URL}/matches/scores`);
            return await this.handleResponse(response);
        } catch (error) {
            console.error('Error fetching cric scores:', error);
            return [];
        }
    },

    async getCurrentMatches() {
        try {
            const response = await fetch(`${API_BASE_URL}/matches/current`);
            return await this.handleResponse(response);
        } catch (error) {
            console.error('Error fetching current matches:', error);
            return [];
        }
    }
};

window.CricketAPI = CricketAPI;
