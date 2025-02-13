import { getCourse, getTeams } from "src/api";

export const load = async ({ params }: {params: {courseId: string}}) => {
    const [
        courseResponse,
        teamsResponse
    ] = await Promise.all([
        getCourse(params.courseId),
        getTeams(params.courseId)
    ])

    return { 
        course: courseResponse.data,
        teams: teamsResponse.data
    }
};