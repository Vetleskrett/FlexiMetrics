<script lang="ts">
	import * as Card from '$lib/components/ui/card/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
	import Plus from 'lucide-svelte/icons/plus';
	import CustomButton from 'src/components/CustomButton.svelte';
	import { Role, type Course } from 'src/types/';

	export let userRole: Role;
	export let courses: Course[];
</script>

<Card.Root class="m-auto mt-16 w-[700px] overflow-hidden p-0">
	<Card.Header class="mb-6 flex flex-row items-center justify-between">
		<div class="flex items-center">
			<img
				width="48"
				height="48"
				src="https://img.icons8.com/fluency/480/knowledge-sharing.png"
				alt="knowledge-sharing"
			/>
			<Card.Title class="ml-4 text-3xl">Courses</Card.Title>
		</div>

		{#if userRole == Role.Teacher}
			<CustomButton href="courses/new" color="green">
				<Plus />
				<p>New</p>
			</CustomButton>
		{/if}
	</Card.Header>
	<Card.Content class="p-0">
		<div class="flex flex-col">
			{#each courses as course}
				<Separator class="w-full" />
				<a
					href="courses/{course.id}"
					class="h-18 flex w-full items-center justify-between p-6 hover:bg-blue-50"
				>
					<h1 class="text-xl">{course.code} - {course.name}</h1>
					<p class="font-semibold text-gray-500">{course.year} {course.semester}</p>
				</a>
			{/each}
		</div>
	</Card.Content>
</Card.Root>
