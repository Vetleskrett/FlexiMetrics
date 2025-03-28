import { getCourse, getTeams, getTeamsProgress } from "src/api.server";
import type { PageServerLoad } from "../$types";

export const load: PageServerLoad = async ({ params }: {params: {courseId: string}}) => {
    const [
        courseResponse,
        teamsResponse,
        teamsProgressResponse
    ] = await Promise.all([
        getCourse(params.courseId),
        getTeams(params.courseId),
        getTeamsProgress(params.courseId)
    ])

    return { 
        course: courseResponse.data,
        teams: teamsResponse.data,
        teamsProgress: teamsProgressResponse.data
    }
};