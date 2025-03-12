<script lang="ts">
	import { page } from '$app/stores';
	import AllTeamCard from 'src/components/team/AllTeamCard.svelte';
	import { type Team, type Course, type StudentToTeam, Role } from 'src/types/';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb';
	import { studentId } from 'src/store';
	import { goto } from '$app/navigation';
	import { postStudentTeam } from 'src/api';

	const courseId = $page.params.courseId;

	async function joinTeam(teamId: string) {
		try {
			await postStudentTeam(teamId, studentId);
			goto(`/student/courses/${courseId}`);
		} catch {
			console.error('Could not join team');
		}
	}

	export let data: {
		course: Course;
		teams: Team[];
	};
</script>

<div class="m-auto mt-4 flex w-max flex-col items-center justify-center gap-10">
	<Breadcrumb.Root class="self-start">
		<Breadcrumb.List>
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/student/courses">Courses</Breadcrumb.Link>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/student/courses/{courseId}"
					>{data.course.code} - {data.course.name}</Breadcrumb.Link
				>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Page>Teams</Breadcrumb.Page>
			</Breadcrumb.Item>
		</Breadcrumb.List>
	</Breadcrumb.Root>
	<div class="flex w-full items-center justify-between">
		<div class="flex items-center">
			<img width="60" height="60" src="https://img.icons8.com/fluency/480/group.png" alt="group" />
			<div>
				<h1 class="ml-4 text-4xl font-semibold">Teams</h1>
			</div>
		</div>
	</div>
	<div class="flex flex-row gap-8">
		<div class="flex w-[700px] flex-col gap-8">
			<AllTeamCard teams={data.teams} {courseId} onJoin={joinTeam} userRole={Role.Student} />
		</div>
	</div>
</div>
