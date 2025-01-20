<script lang="ts">
	import { page } from '$app/stores';
	import AssignmentsCard from 'src/components/AssignmentsCard.svelte';
	import StudentsCard from 'src/components/StudentsCard.svelte';
	import EllipsisVertical from 'lucide-svelte/icons/ellipsis-vertical';
	import Pencil from 'lucide-svelte/icons/pencil';
	import Trash2 from 'lucide-svelte/icons/trash-2';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import TeamsCard from 'src/components/TeamsCard.svelte';
	import TeachersCard from 'src/components/TeachersCard.svelte';
	import * as Breadcrumb from '$lib/components/ui/breadcrumb/index.js';
	import { onMount } from 'svelte';
	import type { Course, Assignment, Teacher } from 'src/types';
	import { Role } from 'src/types';
	import { getCourse, getAssignment } from 'src/api';
	import { userRole } from 'src/store';


	const courseId = $page.params.courseId;

	let course : Course;
	let assignments : Assignment[] = []
	let teachers : Teacher[] = []
	let students : number;
	let teams : number;

	onMount(async () => {
		try{
			course = await getCourse(courseId);
			assignments = await getAssignment(courseId);
			students = course.numStudents ?? 0;
			teams = course.numStudents ?? 0;
			teachers = course.teachers ?? []
		}
		catch(error){
			console.error("Something went wrong!")
		}
	})
</script>

<div class="m-auto mt-4 flex w-max flex-col items-center justify-center gap-10">
	<Breadcrumb.Root class="self-start">
		<Breadcrumb.List>
			<Breadcrumb.Item>
				<Breadcrumb.Link href="/courses">Courses</Breadcrumb.Link>
			</Breadcrumb.Item>
			<Breadcrumb.Separator />
			<Breadcrumb.Item>
				<Breadcrumb.Page>{course?.code || "loading"} - {course?.name || 'loading'}</Breadcrumb.Page>
			</Breadcrumb.Item>
		</Breadcrumb.List>
	</Breadcrumb.Root>
	<div class="flex w-full items-center justify-between">
		<div class="flex items-center">
			<img
				width="60"
				height="60"
				src="https://img.icons8.com/fluency/480/knowledge-sharing.png"
				alt="knowledge-sharing"
			/>
			<div>
				<h1 class="ml-4 text-4xl font-semibold">{course?.code || 'loading'} - {course?.name || 'loading'}</h1>
				<p class="ml-4 font-semibold text-gray-500">{course?.year || 'loading'} {course?.semester || 'loading'}</p>
			</div>
		</div>

		<DropdownMenu.Root>
			<DropdownMenu.Trigger>
				<EllipsisVertical size={32} />
			</DropdownMenu.Trigger>
			<DropdownMenu.Content>
				<DropdownMenu.Item href="/courses/{courseId}/edit">
					<Pencil class="h-4" />
					<p>Edit course</p>
				</DropdownMenu.Item>
				<DropdownMenu.Item>
					<Trash2 class="h-4" />
					<p>Delete course</p>
				</DropdownMenu.Item>
			</DropdownMenu.Content>
		</DropdownMenu.Root>
	</div>
	<div class="flex w-[1080px] flex-row gap-8">
		<div class="flex w-3/5 flex-col gap-8">
			<AssignmentsCard {assignments} {courseId} />
		</div>

		<div class="flex w-2/5 flex-col gap-8">
			{#if $userRole == Role.Teacher}
			<div class="flex flex-row gap-8">
				<StudentsCard {students} {courseId} />
				<TeamsCard {teams} {courseId} />
			</div>
			{/if}
			<TeachersCard {teachers} {courseId} />
		</div>
	</div>
</div>
