const API_BASE_URL = 'http://localhost:5000/api'; // Update port as needed

const CricketAPI = {
    async getLiveMatches() {
        try {
            const response = await fetch(`${API_BASE_URL}/matches/live`);
            return await response.json();
        } catch (error) {
            console.error('Error fetching live matches:', error);
            return [];
        }
    },

    async getUpcomingSchedules() {
        try {
            const response = await fetch(`${API_BASE_URL}/schedules/upcoming`);
            return await response.json();
        } catch (error) {
            console.error('Error fetching schedules:', error);
            return [];
        }
    },

    async getBlogs() {
        try {
            const response = await fetch(`${API_BASE_URL}/blogs`);
            return await response.json();
        } catch (error) {
            console.error('Error fetching blogs:', error);
            return [];
        }
    },

    async searchPlayers(query) {
        try {
            const response = await fetch(`${API_BASE_URL}/players/search-api?name=${query}`);
            return await response.json();
        } catch (error) {
            console.error('Error searching players:', error);
            return [];
        }
    },

    async getSeriesInfo(id) {
        try {
            const response = await fetch(`${API_BASE_URL}/schedules/series/${id}`);
            return await response.json();
        } catch (error) {
            console.error('Error fetching series info:', error);
            return null;
        }
    },

    async getMatchInfo(id) {
        try {
            const response = await fetch(`${API_BASE_URL}/matches/${id}`);
            return await response.json();
        } catch (error) {
            console.error('Error fetching match info:', error);
            return null;
        }
    }
};

window.CricketAPI = CricketAPI;
